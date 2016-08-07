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
                else
                {
                    return null;
                }

                a.DownloadDateTime = DateTime.Now;
                a.Author = "Chefredaktor";

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
                    html = html.Substring(0, html.IndexOf("<table", StringComparison.Ordinal)) + html.Substring(html.IndexOf("</table>", StringComparison.Ordinal) + ("</table>").Length);

                if (html.Contains("<span style=\"font-size: x-small;\">"))
                    html = html.Substring(0, html.IndexOf("<span style=\"font-size: x-small;\">", StringComparison.Ordinal));

                html = "<html><body><p>" + html + "</p></body></html>";

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var str = doc.DocumentNode.InnerText.Trim();
                str = str.Replace("\n\n", "</p><p>");
                str = "<p>" + str + "</p>";

                am.Content.Add(new TextContentModel()
                {
                    Content = HtmlConverter.CreateOnce(am.Feed.Source.PublicBaseUrl).HtmlToParagraph(str)
                });

                am.DownloadDateTime = DateTime.Now;

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
                    .Descendants("div").FirstOrDefault(o => o.GetAttributeValue("class", null) != null &&
                                                                   o.GetAttributeValue("class", null).Contains("post-body"));

                var dateNode = doc.DocumentNode.Descendants("div").FirstOrDefault(o => o.GetAttributeValue("class", null) == "date-header");
                var span = dateNode?.Descendants("span").FirstOrDefault();
                articleModel.PublishDateTime = ParseDateTime(span?.InnerText);

                await AddThemesAsync(articleModel, new[] { "Satire" });
                if (articlenode != null)
                {
                    if (WriteProperties(ref articleModel, articlenode))
                        return true;
                }
                else
                {
                    //happens; sometimes server resonds with service not available (probably some sort of DDOS protection)
                    articleModel.Content.Add(TextHelper.TextToTextModel("Der Artikel konnte nicht heruntergeladen werden"));
                    articleModel.DownloadDateTime = DateTime.Now;
                    return true;
                }
                
                return false;
            });
        }

        private DateTime ParseDateTime(string dateTime)
        {
            if (string.IsNullOrWhiteSpace(dateTime))
                return DateTime.Now;

            var monthConverterDic = new Dictionary<string, string>()
            {
                {"Januar", "01." },
                {"Februar", "02." },
                {"März", "03." },
                {"April","04." },
                {"Mai"," 05." },
                {"Juni", "06." },
                {"Juli", "07." },
                {"August", "08." },
                {"September", "09." },
                {"Oktober", "10." },
                {"November", "11." },
                {"Dezember", "12." }
            };

            DateTime dt;
            var shortDateTime = dateTime.Substring(dateTime.IndexOf(",", StringComparison.Ordinal) + 2);
            shortDateTime = monthConverterDic.Aggregate(shortDateTime, (current, i) => current.Replace(i.Key, i.Value));

            if (DateTime.TryParse(shortDateTime, out dt))
                return dt;
            return DateTime.Now;
        }
    }
}
