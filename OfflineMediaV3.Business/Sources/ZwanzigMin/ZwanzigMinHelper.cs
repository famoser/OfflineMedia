using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.ZwanzigMin.Models;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Sources.ZwanzigMin
{
    public class ZwanzigMinHelper : SingletonBase<ZwanzigMinHelper>, IMediaSourceHelper
    {
        public List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                //removes old header of xml
                feed = feed.Substring(feed.IndexOf(">"));
                feed = feed.Substring(feed.IndexOf("<"));
                feed = HtmlHelper.RemoveXmlLvl(feed);
                feed = HtmlHelper.RemoveXmlLvl(feed);
                feed = HtmlHelper.AddXmlHeaderNode(feed, "channel");

                var serializer = new XmlSerializer(typeof(channel));
                TextReader reader = new StringReader(feed);

                var channel = (channel)serializer.Deserialize(reader);
                if (channel == null)
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed  20 min channel is null after deserialisation");
                else
                    articlelist.AddRange(channel.item.Where(m => CanConvert(m)).Select(fta => FeedToArticleModel(fta)).Where(articleModel => articleModel != null));

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        public ArticleModel EvaluateArticle(string article, SourceConfigurationModel scm)
        {
            throw new NotImplementedException();
        }

        public bool CanConvert(item nfa)
        {
            return nfa.link != null;
        }

        public ArticleModel FeedToArticleModel(item nfa)
        {
            if (nfa == null) return null;

            try
            {
                var a = new ArticleModel
                {
                    Content = new List<ContentModel> { new ContentModel() { Html = nfa.text, Type = ContentType.Html } },
                    LeadImage = new ImageModel()
                    {
                        Html = nfa.topelement_description,
                        Url = new Uri(nfa.pic_bigstory)
                    },
                    PublicUri = new Uri(nfa.link),
                    PublicationTime = DateTime.Parse(nfa.pubDate),
                    SubTitle = nfa.oberzeile,
                    Teaser = nfa.lead,
                    Title = nfa.title,
                };
                //TODO: Author
                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public ArticleModel ArticleToArticleModel(object na)
        {
            throw new NotImplementedException();
        }
    }
}
