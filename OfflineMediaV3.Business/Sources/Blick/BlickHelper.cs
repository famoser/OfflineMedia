using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Logs;
using OfflineMediaV3.Business.Framework.Singleton;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.Blick.Models;

namespace OfflineMediaV3.Business.Sources.Blick
{
    public class BlickHelper : SingletonBase<BlickHelper>, IMediaSourceHelper
    {
        public List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;
            
            try
            {
                var f = JsonConvert.DeserializeObject<feed[]>(feed);
                feed articlefeed = f.FirstOrDefault(i => i.type == "teaser");
                if (articlefeed != null)
                {
                    articlelist.AddRange(articlefeed.items.Where(i => i.type == "teaser")
                        .Select(am => FeedModelToArticleModel(am, scm))
                        .Where(am => am != null));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.EvaluateFeed deserialization failed", ex);
            }
            return articlelist;
        }

        public ArticleModel EvaluateArticle(string article, SourceConfigurationModel scm)
        {
            if (article == null) return null;

            try
            {
                var a = JsonConvert.DeserializeObject<articlefeeditem[]>(article);

                ArticleModel am = ArticleToArticleModel(a, scm);
                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.EvaluateArticle failed", ex);
            }
            return null;
        }

        private ArticleModel FeedModelToArticleModel(feeditem item, SourceConfigurationModel scm)
        {
            try
            {
                var am = new ArticleModel
                {
                    Title = item.title,
                    LogicUri = new Uri(item.targetUrl),
                    PublicUri = new Uri(item.targetUrl.Substring(0, item.targetUrl.IndexOf(".json"))),
                    PublicationTime = item.publicationDate,
                    SubTitle = item.catchword,
                    Teaser = item.lead,
                    LeadImage = new ImageModel() { Url = new Uri(item.img.src) }
                };
                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.FeedModelToArticleModel failed", ex);
                return null;
            }
        }

        public ArticleModel ArticleToArticleModel(articlefeeditem[] na, SourceConfigurationModel scm)
        {
            if (na == null) return null;

            try
            {
                var am = new ArticleModel();

                articlefeeditem category = na.FirstOrDefault(a => a.type == "metadata");
                //if (category != null)



                articlefeeditem body = na.FirstOrDefault(a => a.type == "body");
                if (body != null)
                {
                    var htmlbody = body.items.Where(b => b.type == "text").ToList();
                    am.Content = new List<ContentModel>();

                    for (int i = 0; i < htmlbody.Count(); i++)
                    {
                        am.Content.Add(new ContentModel() { Html = htmlbody[i].txt, Type = ContentType.Html });
                    }
                }

                //TODO: RelatedArticles
                am.RelatedArticles = new List<ArticleModel>();

                articlefeeditem headline = na.FirstOrDefault(a => a.type == "headline");
                if (headline != null && headline.author != null && headline.author.GetType() == typeof(articlefeeditem))
                {
                    articlefeeditem author = headline.author as articlefeeditem;
                    am.Author = author.firstName;
                }

                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.ArticleToArticleModel failed", ex);
                return null;
            }
        }
    }
}
