using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin
{
    public class ZwanzigMinHelper : BaseMediaSourceHelper
    {
        public ZwanzigMinHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        private ArticleModel FeedToArticleModel(Item nfa, FeedModel fcm)
        {
            if (nfa == null || nfa.Text == null) return null;

            return ExecuteSafe(() =>
            {
                var a = ConstructArticleModel(fcm);
                var text = "<p>" + nfa.Text.Replace("\n\n", "</p><p>") + "</p>";

                var paragraphs = HtmlConverter.CreateOnce(fcm.Source.PublicBaseUrl).HtmlToParagraph(text);

                a.Content.Clear();

                if (paragraphs != null && paragraphs.Count > 0)
                    a.Content.Add(
                        new TextContentModel()
                        {
                            Content = paragraphs
                        });
                else if (text.Contains("{{nxpliveticker("))
                    a.Content.Add(
                        TextHelper.TextToTextModel(
                            "Dieser Artikel enthält einen Liveticker. Besuche die Webseite um den Inhalt korrekt darzustellen"));
                else
                    a.Content.Add(
                        TextHelper.TextToTextModel(
                            "Dieser Artikel enthält nicht unterstützter Inhalt. Besuche die Webseite um den Inhalt korrekt darzustellen"));

                a.LeadImage = new ImageContentModel()
                {
                    Url = nfa.PicBigstory
                };
                a.PublicUri = nfa.Link;
                a.PublishDateTime = DateTime.Parse(nfa.PubDate);
                a.SubTitle = nfa.Oberzeile;
                a.Teaser = nfa.Lead;
                a.Title = nfa.Title;
                a.Author = string.IsNullOrEmpty(nfa.Author) ? "20 Minuten" : nfa.Author;

                if (string.IsNullOrWhiteSpace(a.SubTitle) && a.Title == "Die Bilder des Tages")
                {
                    a.SubTitle = "Bildergallerie";
                    //Die Bilder des Tages
                    a.Content.Insert(0,
                        TextHelper.TextToTextModel(
                            "Dieser Artikel enthält eine Bildergallerie. Besuche die Webseite, um den Inhalt korrekt darzustellen"));
                }
                a.Themes.Clear();

                if (nfa.Tags != null)
                    a.AfterSaveFunc = async () =>
                    {
                        foreach (var category in nfa.Tags)
                        {
                            await AddThemesAsync(a, new[] { category.Name });
                        }
                    };
                else
                    a.AfterSaveFunc = async () =>
                    {
                        await AddThemesAsync(a);
                    };

                a.LoadingState = LoadingState.Loaded;

                return a;
            });
        }

        private readonly AsyncLock _customerKeyAsyncLock = new AsyncLock();
        private async Task GetCustomerKey()
        {
            using (await _customerKeyAsyncLock.LockAsync())
            {
                //return early if already executed
                if (CustomerKey != null)
                    return;

                //download js
                var rjs = await DownloadAsync(new Uri("http://m.20min.ch/webapp/js/twenty_min.js"));
                //find customer key
                var appKeyStart = "APPKEY:\"";
                var index = rjs.IndexOf(appKeyStart, StringComparison.Ordinal);
                if (index > 0)
                {
                    var appKeyStartIndex = index + appKeyStart.Length;
                    var endIndex = rjs.IndexOf("\"", appKeyStartIndex, StringComparison.Ordinal);
                    CustomerKey = rjs.Substring(appKeyStartIndex, endIndex - appKeyStartIndex);
                }
                else
                {
                    //fallback, manually read out
                    CustomerKey = "276925d8d98cd956d43cd659051232f7";
                }
            }
        }

        private static string CustomerKey = null;
        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                //find customer key first
                if (CustomerKey == null)
                {
                    await GetCustomerKey();
                }

                var articlelist = new List<ArticleModel>();
                var json = await DownloadAsync(new Uri(feedModel.Source.LogicBaseUrl + feedModel.Url.Replace("CUSTOMERKEY", CustomerKey)));
                if (json == null)
                    return articlelist;

                var feed = GettingStarted.FromJson(json);

                if (feed == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "ZwanzigMinHelper.EvaluateFeed  20 min channel is null after deserialisation", this);
                else
                {
                    foreach (var item in feed.Content.Items.Item)
                    {
                        var model = FeedToArticleModel(item, feedModel);
                        if (model != null)
                            articlelist.Add(model);
                    }
                }

                return articlelist;
            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            throw new NotImplementedException();
        }
    }
}
