using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using HtmlAgilityPack;

namespace Famoser.OfflineMedia.Business.Newspapers.Postillon
{
    public class PostillonHelper : BaseMediaSourceHelper
    {
        public ArticleModel FeedToArticleModel(HtmlNode hn, FeedModel feed)
        {
            if (hn == null) return null;

            try
            {
                var a = new ArticleModel();

                var linknode = hn.ChildNodes.Descendants("a").FirstOrDefault();
                if (linknode != null)
                {
                    a.PublicUri = linknode.GetAttributeValue("href", null);
                    a.LogicUri = a.PublicUri;

                    if (linknode.ChildNodes != null && linknode.ChildNodes.Any())
                    {
                        var img = linknode.ChildNodes[0];
                        a.LeadImage = new ImageContentModel() { Url = img.GetAttributeValue("src", null) };
                    }
                }
                else
                {
                    //newsticker
                    return null;
                }

                var titlenode = hn.ChildNodes.FirstOrDefault(atr => atr.GetAttributeValue("title", null) != null);
                if (titlenode != null)
                    a.Title = titlenode.GetAttributeValue("title", null);

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "PostillonHelper.FeedToArticleModel failed", this, ex);
                return null;
            }
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

                am.Content.Add(new TextContentModel()
                {
                    Content = HtmlConverter.CreateOnce().HtmlToParagraph(doc.DocumentNode.InnerText)
                });

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "PostillonHelper.ArticleToArticleModel failed", this, ex);
                return false;
            }
        }

        public PostillonHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
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
                            var res = FeedToArticleModel(article, feedModel);
                            if (res != null)
                                articlelist.Add(res);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, "PostillonHelper.EvaluateFeed failed", this, ex);
                }
                return articlelist;
            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                var article = await DownloadAsync(articleModel);
                if (article == null) return false;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(article);

                HtmlNode articlenode = doc.DocumentNode
                    .DescendantsAndSelf("div").FirstOrDefault(o => o.GetAttributeValue("class", null) != null &&
                                                                   o.GetAttributeValue("class", null).Contains("post-body"));

                await AddThemesAsync(articleModel, new[] { "Satire" });
                if (articlenode != null)
                {
                    if (WriteProperties(ref articleModel, articlenode))
                        return true;
                }


                articleModel.DownloadDateTime = DateTime.Now;
                return false;
            });
        }
    }
}
