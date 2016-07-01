using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using GalaSoft.MvvmLight.Ioc;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers.Text;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Newspapers.Tamedia.Models;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Data.Helpers;

namespace OfflineMedia.Business.Newspapers.Tamedia
{
    public class TamediaHelper : BaseMediaSourceHelper
    {
        private ArticleModel FeedToArticleModel(Article nfa, FeedModel feedModel)
        {
            if (nfa == null) return null;

            try
            {
                var a = ConstructArticleModel(feedModel);
                a.PublishDateTime = GetCShartTimestamp(nfa.first_published_at);
                a.Title = nfa.title;
                a.SubTitle = null;
                a.Teaser = nfa.lead.Replace("<p>", "").Replace("</p>", "");
                a.LogicUri = feedModel.Source.LogicBaseUrl + "api/articles/" + nfa.id;
                a.PublicUri = feedModel.Source.LogicBaseUrl + nfa.legacy_id;
                
                if (a.LeadImage == null && nfa.picture_medium_url != null)
                {
                    a.LeadImage = new ImageContentModel() { Url = nfa.picture_medium_url };
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

                if (!string.IsNullOrEmpty(nfa.source_annotation))
                {
                    if (string.IsNullOrEmpty(a.Author))
                        a.Author = nfa.source_annotation;
                    else
                        a.Author += " " + nfa.source_annotation;
                }

                if (!string.IsNullOrEmpty(nfa.source))
                {
                    if (string.IsNullOrEmpty(a.Author))
                        a.Author = nfa.source;
                    else
                        a.Author += " " + nfa.source;
                }
                a.Content.Add(new TextContentModel()
                {
                    Content = HtmlConverter.HtmlToParagraph(nfa.text)
                });
                if (nfa.article_elements != null)
                {
                    foreach (var elemnt in nfa.article_elements)
                    {
                        if (elemnt.boxtype == "articles" && elemnt.article_previews != null)
                        {
                            /*
                            a.RelatedArticles = new List<ArticleModel>();
                            for (int i = 0; i < elemnt.article_previews.Count; i++)
                            {
                                a.RelatedArticles.Add(new ArticleModel
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
                            }*/
                        }
                    }
                }

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "TamediaHelper.FeedToArticleModel failed", this, ex);
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

        public TamediaHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
                if (feed == null) return articlelist;

                Feed f = JsonConvert.DeserializeObject<Feed>(feed);
                if (f.category != null && f.category.page_elements != null)
                    foreach (var page in f.category.page_elements)
                    {
                        foreach (var article in page.articles)
                        {
                            var am = FeedToArticleModel(article, feedModel);
                            if (am != null)
                                articlelist.Add(am);
                        }
                    }
                if (f.list != null && f.list.page_elements != null)
                    foreach (var page in f.list.page_elements)
                    {
                        foreach (var article in page.articles)
                        {
                            var am = FeedToArticleModel(article, feedModel);
                            if (am != null)
                                articlelist.Add(am);
                        }
                    }

                return articlelist;

            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                await AddThemesAsync(articleModel);
                return true;
            });
        }
    }
}
