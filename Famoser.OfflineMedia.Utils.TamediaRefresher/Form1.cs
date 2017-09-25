using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Famoser.FrameworkEssentials.Services;
using Famoser.OfflineMedia.Utils.TamediaRefresher.Models.JsonEntities;
using Famoser.OfflineMedia.Utils.TamediaRefresher.Models.TamediaNavigation;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Utils.TamediaRefresher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

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


        private async Task EvaluateSources(ConcurrentStack<SourceEntity> input)
        {
            try
            {

                while (input.TryPop(out var source))
                {
                    if ((int)source.Source >= 20 && (int)source.Source <= 40)
                    {
                        //tamedia sources
                        var httpService = new HttpService();
                        var resp = await httpService.DownloadAsync(new Uri(source.LogicBaseUrl + "navigations?client=webapp"));
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
