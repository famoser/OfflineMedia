using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.Nzz.Models;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Sources
{
    public class HelperModel : SingletonBase<HelperModel>, IMediaSourceHelper
    {
        public List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                var f = JsonConvert.DeserializeObject<NzzFeed>(feed);
                articlelist.AddRange(f.articles.Select(arm => FeedToArticleModel(arm,scm)).Where(am => am != null));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "HelperModel.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        public ArticleModel EvaluateArticle(string article, SourceConfigurationModel scm)
        {
            if (article == null) return null;

            try
            {
                ArticleModel am = ArticleToArticleModel(null, scm);
                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "HelperModel.EvaluateArticle failed", ex);
            }
            return null;
        }

        public ArticleModel FeedToArticleModel(object nfa, SourceConfigurationModel scm)
        {
            if (nfa == null) return null;

            try
            {
                var a = new ArticleModel
                {
                    PublicationTime = new DateTime(),
                    Title = "",
                    SubTitle = "",
                    Teaser = "",
                    LeadImage = new ImageModel(),
                    LogicUri = new Uri(""),
                    PublicUri = new Uri("")
                };

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "HelperModel.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public ArticleModel ArticleToArticleModel(object na, SourceConfigurationModel scm)
        {
            if (na == null) return null;

            try
            {
                var a = new ArticleModel();

                a.Author = "";
                a.Content = new List<ContentModel>();
                a.RelatedArticles = new List<ArticleModel>();

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "HelperModel.ArticleToArticleModel failed", ex);
                return null;
            }
        }
    }
}
