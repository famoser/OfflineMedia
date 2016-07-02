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
            if (nfa == null) return null;

            return ExecuteSafe(() =>
            {

                var a = ConstructArticleModel(fcm);
                a.Content.Add(
                    new TextContentModel()
                    {
                        Content = HtmlConverter.HtmlToParagraph(nfa.text)
                    });

                a.LeadImage = new ImageContentModel()
                {
                    Url = nfa.pic_bigstory,
                    Text = TextConverter.TextToTextModel(nfa.topelement_description)
                };
                a.PublicUri = nfa.link;
                a.PublishDateTime = DateTime.Parse(nfa.pubDate);
                a.SubTitle = nfa.oberzeile;
                a.Teaser = nfa.lead;
                a.Title = nfa.title;
                a.Author = nfa.author;
                
                a.AfterSaveFunc = async () =>
                {
                    await AddThemesAsync(a, new[] { nfa.category });
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
