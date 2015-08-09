using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.Tamedia.Models;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Sources.Tamedia
{
    public class TamediaHelper : SingletonBase<TamediaHelper>, IMediaSourceHelper
    {
        public List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                Feed f = JsonConvert.DeserializeObject<Feed>(feed);
                foreach (var page in f.category.page_elements)
                {
                    articlelist.AddRange(page.articles.Select(ar => FeedToArticleModel(ar, scm)).Where(am => am != null));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "TamediaHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        public ArticleModel EvaluateArticle(string article, SourceConfigurationModel scm)
        {
            if (article == null) return null;

            try
            {
                ArticleModel am = ArticleToArticleModel(null);
                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "TamediaHelper.EvaluateArticle failed", ex);
            }
            return null;
        }

        public ArticleModel FeedToArticleModel(Article nfa, SourceConfigurationModel scm)
        {
            if (nfa == null) return null;

            try
            {
                var a = new ArticleModel
                {
                    PublicationTime = GetCShartTimestamp(nfa.first_published_at),
                    Title = nfa.title,
                    SubTitle = null,
                    Teaser = nfa.lead.Replace("<p>","").Replace("</p>",""),
                    //TODO: Themes
                    //Themes = new Theme[1] { ConvertToEnum.ConvertToTheme("") },
                    LogicUri = new Uri(scm.LogicBaseUrl + "api/articles/" + nfa.id),
                    PublicUri = new Uri(scm.PublicBaseUrl + nfa.legacy_id)
                };

                if (nfa.top_element != null)
                {
                    if (nfa.top_element.boxtype == "picture")
                    {
                        a.LeadImage = new ImageModel() { Url = new Uri(nfa.picture_medium_url), Html = nfa.text };
                    }
                }
                if (nfa.authors != null)
                {
                    foreach (var item in nfa.authors)
                    {
                        if (!string.IsNullOrEmpty(a.Author))
                            a.Author += ", ";
                        a.Author += item.name;
                    }
                }

                a.Content = new List<ContentModel> 
                { 
                    new ContentModel() {
                        Html = nfa.text,
                        Type = ContentType.Html
                    }
                };
                if (nfa.article_elements != null)
                {
                    foreach (var elemnt in nfa.article_elements)
                    {
                        if (elemnt.boxtype == "articles" && elemnt.article_previews != null)
                        {
                            a.RelatedArticles = new List<ArticleModel>();
                            for (int i = 0; i < elemnt.article_previews.Count; i++)
                            {
                                a.RelatedArticles.Add(new ArticleModel()
                                {
                                    LogicUri = new Uri(scm.LogicBaseUrl + "api/articles/" + elemnt.article_previews[i].id),
                                    PublicUri = new Uri(scm.PublicBaseUrl + elemnt.article_previews[i].legacy_id),
                                    PublicationTime = new DateTime(elemnt.article_previews[i].first_published_at),
                                    Title = elemnt.article_previews[i].title,
                                    SubTitle = null,
                                    Teaser = elemnt.article_previews[i].lead,
                                    LeadImage = new ImageModel() { Url = new Uri(elemnt.article_previews[i].picture_medium_url) }
                                });

                                if (elemnt.article_previews[i].authors != null)
                                {
                                    foreach (var item in elemnt.article_previews[i].authors)
                                    {
                                        if (!string.IsNullOrEmpty(a.Author))
                                            a.RelatedArticles[i].Author += ", ";
                                        a.RelatedArticles[i].Author += item.name;
                                    }
                                }
                            }
                        }
                    }
                }

                a.State = ArticleState.Loaded;

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "TamediaHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public ArticleModel ArticleToArticleModel(object na)
        {
            if (na == null) return null;

            try
            {
                var a = new ArticleModel();
                return a;
                //throw new NotImplementedException();


            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "TamediaHelper.ArticleToArticleModel failed", ex);
                return null;
            }
        }

        #region Helpers
        /// <summary>
        /// Documented at https://github.com/flot/flot/blob/master/API.md (look for public static int GetJavascriptTimestamp(System.DateTime input)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime GetCShartTimestamp(long input)
        {
            TimeSpan ts = TimeSpan.FromSeconds(input);
            DateTime dt = DateTime.Parse("1/1/1970");
            dt += ts;
            return dt;
        }


#endregion
    }
}
