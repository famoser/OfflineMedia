using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Famoser.FrameworkEssentials.Logging;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Newspapers.ZwanzigMin.Models;

namespace OfflineMedia.Business.Newspapers.ZwanzigMin
{
    public class ZwanzigMinHelper : BaseMediaSourceHelper
    {
        public override async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scm, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
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
                    LogHelper.Instance.Log(LogLevel.Error,"ZwanzigMinHelper.EvaluateFeed  20 min channel is null after deserialisation", this);
                else
                {
                    foreach (var item in channel.item)
                    {
                        if (CanConvert(item))
                        {
                            var model = await FeedToArticleModel(item, fcm);
                            if (model != null)
                                articlelist.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error,"ZwanzigMinHelper.EvaluateFeed failed", this, ex);
            }
            return articlelist;
        }

        public override bool NeedsToEvaluateArticle()
        {
            return false;
        }

        public bool CanConvert(item nfa)
        {
            return nfa.link != null;
        }

        public override Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            throw new NotImplementedException();
        }

        public override bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            throw new NotImplementedException();
        }

        private async Task<ArticleModel> FeedToArticleModel(item nfa, FeedConfigurationModel fcm)
        {
            if (nfa == null) return null;

            try
            {
                var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                var a = new ArticleModel
                {
                    Content = new List<ContentModel> { new ContentModel() { Html = nfa.text, ContentType = ContentType.Html } },
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
                    Author = nfa.author,
                    Themes = new List<ThemeModel>()
                    {
                        await repo.GetThemeModelFor(nfa.category),
                        await repo.GetThemeModelFor(fcm.Name)
                    }
                };

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error,"ZwanzigMinHelper.FeedToArticleModel failed", this, ex);
                return null;
            }
        }
    }
}
