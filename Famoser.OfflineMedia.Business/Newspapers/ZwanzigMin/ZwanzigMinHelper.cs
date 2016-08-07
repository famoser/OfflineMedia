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

namespace Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin
{
    public class ZwanzigMinHelper : BaseMediaSourceHelper
    {
        public ZwanzigMinHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        private bool CanConvert(item nfa)
        {
            return nfa.link != null;
        }

        private ArticleModel FeedToArticleModel(item nfa, FeedModel fcm)
        {
            if (nfa == null || nfa.link.Contains("tilllate")) return null;

            return ExecuteSafe(() =>
            {
                /*
                 * <text>
                        <![CDATA[
                            <!--{{nxpliveticker('57833145ab5c371ee8000001')}}--> 
                        ]]>
                    </text>
                */

                var a = ConstructArticleModel(fcm);
                nfa.text = "<p>" + nfa.text.Replace("\n\n", "</p><p>") + "</p>";

                var paragraphs = HtmlConverter.CreateOnce(fcm.Source.PublicBaseUrl).HtmlToParagraph(nfa.text);

                if (paragraphs != null && paragraphs.Count > 0)
                    a.Content.Add(
                        new TextContentModel()
                        {
                            Content = paragraphs
                        });
                else if (nfa.text.Contains("{{nxpliveticker("))
                    a.Content.Add(
                        TextHelper.TextToTextModel(
                            "Dieser Artikel enthält einen Liveticker. Besuche die Webseite um den Inhalt korrekt darzustellen"));
                else
                    a.Content.Add(
                        TextHelper.TextToTextModel(
                            "Dieser Artikel enthält nicht unterstützter Inhalt. Besuche die Webseite um den Inhalt korrekt darzustellen"));

                a.LeadImage = new ImageContentModel()
                {
                    Url = nfa.pic_bigstory,
                    Text = TextHelper.TextToTextModel(nfa.topelement_description)
                };
                a.PublicUri = nfa.link;
                a.PublishDateTime = DateTime.Parse(nfa.pubDate);
                a.SubTitle = nfa.oberzeile;
                a.Teaser = nfa.lead;
                a.Title = nfa.title;
                a.Author = string.IsNullOrEmpty(nfa.author) ? "20 Minuten" : nfa.author;

                if (string.IsNullOrWhiteSpace(a.SubTitle) && a.Title == "Die Bilder des Tages")
                {
                    a.SubTitle = "Bildergallerie";
                    //Die Bilder des Tages
                    a.Content.Insert(0,
                        TextHelper.TextToTextModel(
                            "Dieser Artikel enthält eine Bildergallerie. Besuche die Webseite, um den Inhalt korrekt darzustellen"));
                }
                if (nfa.category != null)
                    a.AfterSaveFunc = async () =>
                    {
                        await AddThemesAsync(a, new[] { nfa.category });
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

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
                if (feed == null)
                    return articlelist;

                //removes old header of xml
                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                feed = HtmlHelper.RemoveXmlLvl(feed);
                feed = HtmlHelper.RemoveXmlLvl(feed);
                feed = HtmlHelper.AddXmlHeaderNode(feed, "channel");

                var serializer = new XmlSerializer(typeof(channel));
                TextReader reader = new StringReader(feed);

                var channel = (channel)serializer.Deserialize(reader);
                if (channel == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "ZwanzigMinHelper.EvaluateFeed  20 min channel is null after deserialisation", this);
                else
                {
                    foreach (var item in channel.item)
                    {
                        if (CanConvert(item))
                        {
                            var model = FeedToArticleModel(item, feedModel);
                            if (model != null)
                                articlelist.Add(model);
                        }
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
