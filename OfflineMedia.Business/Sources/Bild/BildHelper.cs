using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources.Bild.Models.Feed;
using OfflineMedia.Business.Sources.ZwanzigMin.Models;
using OfflineMedia.Common.Framework.Logs;

namespace OfflineMedia.Business.Sources.Bild
{
    public class BildHelper : IMediaSourceHelper
    {
        public async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                var rootObj = JsonConvert.DeserializeObject<RootObject>(feed);
                
                if (rootObj == null)
                    LogHelper.Instance.Log(LogLevel.Error, this, "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation");
                else
                {
                    foreach (var children in rootObj.__childNodes__)
                    {
                        var article = await FeedToArticleModel(children, fcm);
                        if (article != null)
                            articlelist.Add(article);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        private async Task<ArticleModel> FeedToArticleModel(ChildNode item, FeedConfigurationModel fcm)
        {
            //if (nfa == null || nfa.Link.Contains("/video/video"))
            //    return null;

            //try
            //{
            //    var title = nfa.Title.Substring(0, nfa.Title.IndexOf(":", StringComparison.Ordinal));
            //    var subTitle = nfa.Title.Substring(nfa.Title.IndexOf(":", StringComparison.Ordinal) + 1);
            //    var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
            //    var link = nfa.Link;
            //    if (link.Contains("#ref=rss"))
            //        link = link.Replace("#ref=rss", "");
            //    var a = new ArticleModel
            //    {
            //        Title = title,
            //        SubTitle = subTitle,
            //        Teaser = nfa.Description,
            //        PublicationTime = DateTime.Parse(nfa.PubDate),
            //        PublicUri = new Uri(nfa.Link),
            //        LogicUri = new Uri(link),
            //        Themes = new List<ThemeModel>()
            //        {
            //            await repo.GetThemeModelFor(nfa.Category)
            //        }
            //    };

            //    if (nfa.Enclosure != null && nfa.Enclosure.Type != null && nfa.Enclosure.Type.Contains("image"))
            //    {
            //        var url = nfa.Enclosure.Url;
            //        if (url.Contains("thumbsmall"))
            //            url = url.Replace("thumbsmall", "breitwandaufmacher");
            //        a.LeadImage = new ImageModel() { Url = new Uri(url) };
            //    }

            //    return a;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.FeedToArticleModel failed", ex);
            //    return null;
            //}
            return null;
        }

        public bool NeedsToEvaluateArticle()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
