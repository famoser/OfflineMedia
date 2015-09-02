using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Ioc;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.ZwanzigMin.Models;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Singleton;
using OfflineMediaV3.Common.Helpers;

namespace OfflineMediaV3.Business.Sources.ZwanzigMin
{
    public class ZwanzigMinHelper : SingletonBase<ZwanzigMinHelper>, IMediaSourceHelper
    {
        public async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scm, FeedConfigurationModel fcm)
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
                    LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed  20 min channel is null after deserialisation");
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
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        public bool NeedsToEvaluateArticle()
        {
            return false;
        }

        public bool CanConvert(item nfa)
        {
            return nfa.link != null;
        }

        public Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            throw new NotImplementedException();
        }

        public bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            throw new NotImplementedException();
        }

        public List<string> GetKeywords(ArticleModel articleModel)
        {
            var part1 = TextHelper.Instance.GetImportantWords(articleModel.Title);
            var part2 = TextHelper.Instance.GetImportantWords(articleModel.SubTitle);

            return TextHelper.Instance.FusionLists(part1, part2);
        }

        public async Task<ArticleModel> FeedToArticleModel(item nfa, FeedConfigurationModel fcm)
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
