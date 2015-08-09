using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Sources.Postillon
{
    public class PostillonHelper : SingletonBase<PostillonHelper>, IMediaSourceHelper
    {
        public List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(feed);

                List<HtmlNode> articles = doc.DocumentNode
                    .DescendantsAndSelf("div")
                    .Where(
                        o => o.GetAttributeValue("class", null) != null &&
                        o.GetAttributeValue("class", null).Contains("post-body")
                        )
                    .ToList();

                if (articles != null && articles.Any())
                {
                    foreach (var article in articles)
                    {
                        var res = FeedToArticleModel(article, scm);
                        if (res != null)
                            articlelist.Add(res);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "PostillonHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        public ArticleModel EvaluateArticle(string article, SourceConfigurationModel scm)
        {
            if (article == null) return null;

            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(article);

                HtmlNode articlenode = doc.DocumentNode
                    .DescendantsAndSelf("div").FirstOrDefault(o => o.GetAttributeValue("class", null) != null &&
                                                                   o.GetAttributeValue("class", null).Contains("post-body"));

                if (articlenode != null)
                {
                    return ArticleToArticleModel(articlenode, scm);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "PostillonHelper.EvaluateArticle failed", ex);
            }
            return null;
        }

        public ArticleModel FeedToArticleModel(HtmlNode hn, SourceConfigurationModel scm)
        {
            if (hn == null) return null;

            try
            {
                var a = new ArticleModel();

                var linknode = hn.ChildNodes.Descendants("a").FirstOrDefault();
                if (linknode != null)
                {
                    a.PublicUri = new Uri(linknode.GetAttributeValue("href", null));
                    a.LogicUri = a.PublicUri;

                    if (linknode.ChildNodes != null && linknode.ChildNodes.Any())
                    {
                        var img = linknode.ChildNodes[0];
                        a.LeadImage = new ImageModel() { Url = new Uri(img.GetAttributeValue("src", null)) };
                    }
                }
                else
                {
                    //newsticker
                    a.State =ArticleState.Loaded;
                    return null;
                }

                var titlenode = hn.ChildNodes.FirstOrDefault(atr => atr.GetAttributeValue("title", null) != null);
                if (titlenode != null)
                    a.Title = titlenode.GetAttributeValue("title", null);

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "PostillonHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public ArticleModel ArticleToArticleModel(HtmlNode na, SourceConfigurationModel scm)
        {
            if (na == null) return null;

            try
            {
                var a = new ArticleModel();
                string html  = na.InnerHtml;
                
                if (html.Contains("<table"))
                    html = html.Substring(0, html.IndexOf("<table")) + html.Substring(html.IndexOf("</table>") + ("</table>").Length);

                if (html.Contains("<span style=\"font-size: x-small;\">"))
                    html = html.Substring(0, html.IndexOf("<span style=\"font-size: x-small;\">"));

                html = "<html>" + html + "</html>";

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                a.Content = new List<ContentModel> { new ContentModel() { Html = doc.DocumentNode.InnerText, Type = ContentType.Html } };

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "PostillonHelper.ArticleToArticleModel failed", ex);
                return null;
            }
        }
    }
}
