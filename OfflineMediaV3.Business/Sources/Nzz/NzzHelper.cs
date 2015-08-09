using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Logs;
using OfflineMediaV3.Business.Framework.Singleton;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources.Nzz.Models;

namespace OfflineMediaV3.Business.Sources.Nzz
{
    public class NzzHelper : SingletonBase<NzzHelper>, IMediaSourceHelper
    {

        public List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed != null)
            {
                try
                {
                    var f = JsonConvert.DeserializeObject<NzzFeed>(feed);
                    articlelist.AddRange(f.articles.Select(am => FeedToArticleModel(am,scm)).Where(am => am != null));
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateFeed failed", ex);
                }
            }
            return articlelist;
        }

        public ArticleModel EvaluateArticle(string article, SourceConfigurationModel scm)
        {
            if (article == null) return null;

            try
            {
                article = article.Replace("[[]]", "[]");
                var a = JsonConvert.DeserializeObject<NzzArticle>(article);
                ArticleModel am = ArticleToArticleModel(a);
                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.EvaluateArticle failed", ex);
                return null;
            }
        }

        public ArticleModel FeedToArticleModel(NzzFeedArticle nfa, SourceConfigurationModel scm)
        {
            if (nfa == null) return null;

            try
            {
                var a = new ArticleModel
                {
                    PublicationTime = nfa.publicationDateTime,
                    Title = nfa.title,
                    SubTitle = nfa.subTitle,
                    Teaser = nfa.teaser,
                };


                a.LeadImage = LeadImageToImage(nfa.leadImage);

                a.LogicUri = new Uri(scm.LogicBaseUrl + nfa.path.Substring(1));
                string guid = nfa.path.Substring(nfa.path.LastIndexOf("/") + 1);
                a.PublicUri = new Uri(scm.PublicBaseUrl + guid);

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public ArticleModel ArticleToArticleModel(NzzArticle na)
        {
            if (na != null)
            {
                try
                {
                    var a = new ArticleModel();


                    a.Content = new List<ContentModel>();
                    for (int i = 0; i < na.body.Length; i++)
                    {
                        if (na.body[i].style == "h4")
                            na.body[i].style = "h2";
                        if (na.body[i].style == "h3")
                            na.body[i].style = "h1";
                        string starttag = "<" + na.body[i].style + ">";
                        string endtag = "</" + na.body[i].style + ">";
                        a.Content.Add(new ContentModel() { Type = Enums.ContentType.Html, Html = starttag + na.body[i].text + endtag });
                    }

                    // TODO: RelatedArticles, Author

                    return a;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.ArticleToArticleModel deserialization failed", ex);
                    return null;
                }
            }
            return null;
        }

        private ImageModel LeadImageToImage(NzzLeadImage li)
        {
            if (li != null)
            {
                try
                {
                    var img = new ImageModel {Html = li.caption, Author = li.source };
                    string uri = li.path.Replace("%width%", "640").Replace("%height%", "360").Replace("%format%", "text");
                    img.Url = new Uri(uri);
                    return img;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, this, "NzzHelper.LeadImageToImage deserialization failed", ex);
                    return null;
                }
            }
            return null;
        }
    }
}
