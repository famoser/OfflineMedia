using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using HtmlAgilityPack;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Singleton;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources.Postillon
{
    public class PostillonHelper : BaseMediaSourceHelper
    {
#pragma warning disable 1998
        public override async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scm, FeedConfigurationModel fcm)
#pragma warning restore 1998
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
                    a.State = ArticleState.Loaded;
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
        
        public override async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            if (article == null) return new Tuple<bool, ArticleModel>(false, am);

            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(article);

                HtmlNode articlenode = doc.DocumentNode
                    .DescendantsAndSelf("div").FirstOrDefault(o => o.GetAttributeValue("class", null) != null &&
                                                                   o.GetAttributeValue("class", null).Contains("post-body"));

                var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                am.Themes = new List<ThemeModel>()
                {
                    await repo.GetThemeModelFor(am.FeedConfiguration.Name),
                    await repo.GetThemeModelFor("Satire"),
                };

                if (articlenode != null)
                {
                    if (WriteProperties(ref am, articlenode))
                        return new Tuple<bool, ArticleModel>(true, am);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "PostillonHelper.EvaluateArticle failed", ex);
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        public new bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Content = evaluatedArticle.Content;
            original.Themes = evaluatedArticle.Themes;
            original.Author = evaluatedArticle.Author;
            return true;
        }

        public bool WriteProperties(ref ArticleModel am, HtmlNode na)
        {
            if (na == null) return false;

            try
            {
                string html = na.InnerHtml;

                if (html.Contains("<table"))
                    html = html.Substring(0, html.IndexOf("<table")) + html.Substring(html.IndexOf("</table>") + ("</table>").Length);

                if (html.Contains("<span style=\"font-size: x-small;\">"))
                    html = html.Substring(0, html.IndexOf("<span style=\"font-size: x-small;\">"));

                html = "<html>" + html + "</html>";

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                am.Content = new List<ContentModel> { new ContentModel() { Html = doc.DocumentNode.InnerText, ContentType = ContentType.Html } };

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "PostillonHelper.ArticleToArticleModel failed", ex);
                return false;
            }
        }
    }
}
