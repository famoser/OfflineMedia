using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Famoser.FrameworkEssentials.Services;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Data.Enums;
using Famoser.OfflineMedia.Utils.TamediaRefresher.Models.JsonEntities;
using Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TamediaNavigation;
using Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TwentyMinSitemap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Famoser.OfflineMedia.Utils.TamediaRefresher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ReadOutSourcesJson();
        }

        private void ReadOutSourcesJson()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Assets\Sources.json");
            var json = File.ReadAllText(path);
            var beautifulJson = JToken.Parse(json).ToString(Formatting.Indented);
            inputTextBox.Text = beautifulJson;
        }

        private const string TwentyMinCustomerKey = "276925d8d98cd956d43cd659051232f7";

        private async void startButton_Click(object sender, EventArgs e)
        {
            var sourceModels = JsonConvert.DeserializeObject<List<SourceEntity>>(inputTextBox.Text);

            var stack = new ConcurrentStack<SourceEntity>(sourceModels);

            var maxThreads = 10;
            var tasks = new Task[maxThreads];
            for (int i = 0; i < maxThreads; i++)
            {
                tasks[i] = EvaluateSources(stack);
            }

            await Task.WhenAll(tasks);

            outputTextBox.Text = JsonConvert.SerializeObject(
                sourceModels,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }
            );
        }

        private readonly HttpService _httpService = new HttpService();

        private async Task EvaluateSources(ConcurrentStack<SourceEntity> input)
        {
            try
            {

                while (input.TryPop(out var source))
                {
                    if ((int)source.Source >= 20 && (int)source.Source <= 40)
                    {
                        //tamedia sources
                        var resp = await _httpService.DownloadAsync(new Uri(source.LogicBaseUrl + "navigations?client=webapp"));
                        var json = await resp.GetResponseAsStringAsync();

                        var model = TamediaNavigation.FromJson(json);
                        var existing = source.Feeds.ToList();
                        var newList = new List<FeedEntity>();
                        foreach (var navigation in model.Navigations)
                        {
                            if (navigation.CategoryPreview != null)
                            {
                                var found = FindAndRemove(existing, navigation.CategoryPreview.Name) ?? new FeedEntity()
                                {
                                    Guid = Guid.NewGuid(),
                                    Name = navigation.CategoryPreview.Name
                                };

                                //correct category
                                found.Url = "categories/" + navigation.CategoryPreview.Id;
                                newList.Add(found);
                            }
                        }

                        //skip adding of front because currently it cannot be processed
                        if (false)
                        {
                            //add / correct special front navigation
                            var front = FindAndRemove(existing, "Front") ?? new FeedEntity()
                            {
                                Guid = Guid.NewGuid(),
                                Name = "Front"
                            };

                            //correct category
                            front.Url = "fronts/mobile";
                            newList.Insert(0, front);
                        }

                        //to output
                        source.Feeds.Clear();
                        foreach (var feedEntity in newList)
                        {
                            source.Feeds.Add(feedEntity);
                        }
                    }
                    else if (source.Source == Sources.ZwanzigMin)
                    {
                        var resp = await _httpService.DownloadAsync(new Uri("http://api.20min.ch/feed/sitemap?&key=" + TwentyMinCustomerKey + "&json&host=m.20min.ch&lang=de"));
                        var json = await resp.GetResponseAsStringAsync();

                        var feedDic = new Dictionary<string, FeedEntity>();
                        foreach (var sourceFeed in source.Feeds)
                        {
                            feedDic.Add(sourceFeed.Name, sourceFeed);
                        }

                        source.Feeds.Clear();
                        source.LogicBaseUrl = "http://api.20min.ch/feed";
                        var logicLength = source.LogicBaseUrl.Length;

                        var model = GettingStarted.FromJson(json);
                        foreach (var contentItem in model.Content.Items.Item.Where(c => !string.IsNullOrEmpty(c.Category) && c.Type == "view"))
                        {
                            FeedEntity item = null;
                            if (feedDic.ContainsKey(contentItem.Category))
                            {
                                item = feedDic[contentItem.Category];
                            }
                            else
                            {
                                item = new FeedEntity { Name = contentItem.Category, Guid = Guid.NewGuid() };
                            }
                            item.Url = contentItem.FeedFullContentUrl.Substring(logicLength).Replace(TwentyMinCustomerKey, "CUSTOMERKEY");
                            source.Feeds.Add(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private FeedEntity FindAndRemove(List<FeedEntity> feeds, string name)
        {
            //find existing
            FeedEntity found = null;
            foreach (var feedEntity in feeds)
            {
                if (feedEntity.Name == name)
                {
                    //winrar
                    found = feedEntity;
                }
            }
            if (found != null)
            {
                feeds.Remove(found);
            }
            return found;
        }
    }
}
