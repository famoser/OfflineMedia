using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.Zeit.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using HtmlAgilityPack;

namespace Famoser.OfflineMedia.Business.Newspapers.Zeit
{
    public class ZeitHelper : BaseMediaSourceHelper
    {
        public ZeitHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var feed = await DownloadAsync(feedModel);

                var articlelist = new List<ArticleModel>();
                if (feed == null)
                    return articlelist;

                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                if (!RepairFeedXml(ref feed))
                    return articlelist;

                var zeitFeed = XmlHelper.Deserialize<Feed>(feed);
                if (zeitFeed == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "ZeitHelper.EvaluateFeed failed: Feed is null after deserialisation", this);
                else
                {
                    foreach (var item in zeitFeed.References)
                    {
                        var article = FeedReferenceToArticleModel(item, feedModel);
                        if (article != null)
                            articlelist.Add(article);
                    }
                }
                return articlelist;
            });
        }

        private bool RepairFeedXml(ref string xml)
        {
            try
            {
                var hrefPattern = "[^\"]";
                var clusters = XmlHelper.GetNodes(xml, "block");
                if (!clusters.Any())
                    return false;

                xml = clusters.Aggregate((c, s) => c + s);

                //remove namespaces
                xml = Regex.Replace(xml, "([A-Za-z])\\w+:", " ");

                //remove incorrect names
                xml = Regex.Replace(xml, "__([A-Za-z])+__=\"([a-z0-9-])+\"", "");

                //remove incorrect link nodes
                var matchList = Regex.Matches(xml, "href=\"(" + hrefPattern + ")+\\.\\.\\.([ ])+[^\"]");
                foreach (var matchItem in matchList)
                {
                    var match = matchItem as Match;
                    if (match != null)
                        xml = xml.Replace(match.Value, match.Value.Substring(match.Value.Length - 2));
                }

                //remove double hrefs
                var matchList2 = Regex.Matches(xml, "href=\"(" + hrefPattern + ")+([^<>])+href=\"(" + hrefPattern + ")+\"");
                foreach (var matchItem in matchList2)
                {
                    var match = matchItem as Match;
                    //remove first href
                    if (match != null)
                    {
                        var firstHref = match.Value.Substring(0, match.Value.Substring("href=\"".Length + 1).IndexOf("\"", StringComparison.Ordinal) + "href=\"".Length + 1 + 1);
                        xml = xml.Replace(firstHref, "");
                    }
                }

                xml = "<feed>" + xml + "</feed>";
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "RepairFeedXml in Zeit failed", this, ex);
                return false;
            }
        }

        private ArticleModel FeedToArticleModel(Container feedArticle, FeedModel fcm)
        {
            return ExecuteSafe(() =>
            {
                if (feedArticle?.Block == null) return null;

                //block articles from other domains and videos
                if (!feedArticle.Block.Href.Contains("xml.zeit.de/") ||
                    feedArticle.Block.Href.Contains("zeit.de/video/"))
                    return null;

                var link = "http://" + feedArticle.Block.Href.Trim().Substring(2);
                var pubLink = link.Replace("xml.zeit.de", "zeit.de");
                var a = ConstructArticleModel(fcm);
                a.Title = feedArticle.Block.Title;
                a.SubTitle = feedArticle.Block.Supertitle;
                a.Teaser = feedArticle.Block.Description ?? feedArticle.Block.Text;
                a.Author = feedArticle.Block.Author.Any() ? feedArticle.Block._Author.Trim() : "Zeit";
                a.Content.Clear();
                a.Content.Add(TextHelper.TextToTextModel(feedArticle.Block.Text));

                DateTime dateTime;
                a.PublishDateTime = DateTime.TryParse(feedArticle.Block.Publicationdate, out dateTime) ? dateTime : DateTime.Now;

                a.PublicUri = pubLink;
                a.LogicUri = link;

                if (feedArticle.Block.Image != null && feedArticle.Block.Image.Baseid != null &&
                    !string.IsNullOrEmpty(feedArticle.Block.Image.Type))
                {
                    var url = feedArticle.Block.Image.Baseid.Trim();
                    if (url.Contains("//xml.zeit.de"))
                    {
                        a.LeadImage = new ImageContentModel()
                        {
                            Url = "http://" + url.Replace("//xml.zeit.de", "zeit.de") + "cinema__940x403"
                        };
                    }
                }

                a.AfterSaveFunc = async () =>
                {
                    a.Themes.Clear();
                    await AddThemesAsync(a, new[] { feedArticle.Block.Ressort, feedArticle.Block.Genre });
                };

                return a;
            });
        }

        private ArticleModel FeedReferenceToArticleModel(Reference feedArticle, FeedModel fcm)
        {
            return ExecuteSafe(() =>
            {
                if (feedArticle == null || feedArticle.Year == "1900") return null;

                //is taken care of in validLinks
                var ignoreLinks = new[]
                {
                    "http://xml.zeit.de/thema",
                    "http://xml.zeit.de/doctor",
                };
                var validLinks = new[]
                {
                    "http:\\/\\/zeit.de\\/(([a-z\\-])+\\/)*([0-9]){4}[-/]([0-9]){2}"
                };

                //block articles from other domains and videos
                if (!feedArticle.Href.Contains("xml.zeit.de/") ||
                    feedArticle.Href.Contains("zeit.de/video/"))
                    return null;

                var link = "http://" + feedArticle.Href.Trim().Substring(2);
                var pubLink = link.Replace("xml.zeit.de", "zeit.de");

                var boo = false;
                foreach (var validLink in validLinks)
                {
                    boo |= Regex.IsMatch(pubLink, validLink);
                    if (boo)
                        break;
                }
                if (!boo)
                    return null;

                var a = ConstructArticleModel(fcm);
                a.Title = feedArticle.Title;
                a.SubTitle = feedArticle.Supertitle;
                a.Teaser = feedArticle.Description ?? feedArticle.Text;
                a.Author = feedArticle.Author?.Display_name ?? (feedArticle._Author?.Trim() ?? "Zeit");
                a.Content.Clear();
                a.Content.Add(TextHelper.TextToTextModel(feedArticle.Text));

                DateTime dateTime;
                a.PublishDateTime = DateTime.TryParse(feedArticle.Publicationdate, out dateTime) ? dateTime : DateTime.Now;

                a.PublicUri = pubLink;
                a.LogicUri = link;

                if (feedArticle.Image != null && feedArticle.Image.Baseid != null &&
                    !string.IsNullOrEmpty(feedArticle.Image.Type))
                {
                    var url = feedArticle.Image.Baseid.Trim();
                    if (url.Contains("//xml.zeit.de"))
                    {
                        a.LeadImage = new ImageContentModel()
                        {
                            Url = "http://" + url.Replace("//xml.zeit.de", "zeit.de") + "cinema__940x403"
                        };
                    }
                }

                a.AfterSaveFunc = async () =>
                {
                    a.Themes.Clear();
                    await AddThemesAsync(a, new[] { feedArticle.Ressort });
                };

                return a;
            });
        }

        private string RepairArticleXml(string xml)
        {
            var xml1 = XmlHelper.GetSingleNode(xml, "body");
            if (xml1 != null)
            {
                var xml2 = XmlHelper.GetSingleNode(xml1, "division");
                if (xml2 != null)
                {
                    xml2 = xml2.Replace("intertitle", "h2");
                    return xml2;
                }

                var xml3 = XmlHelper.GetSingleNode(xml1, "text");
                if (xml3 != null && xml3 != "<text ")
                {
                    xml3 = xml3.Replace("intertitle", "h2");
                    return xml3;
                }

                var xml4 = XmlHelper.GetSingleNode(xml, "teaser");
                if (xml4 != null)
                {
                    xml4 = XmlHelper.GetSingleNode(xml4, "text");
                    xml4 = xml4.Replace("<text>", "<p>");
                    return xml4.Replace("</text>", "</p>");
                }

                //teaser
            }
            return null;
        }

        private DateTime? GetArticleDate(string xml)
        {
            try
            {
                if (xml.Contains("date_last_published"))
                {
                    var dateStr = xml.Substring(xml.IndexOf("date_last_published", StringComparison.Ordinal));
                    dateStr = dateStr.Substring(dateStr.IndexOf(">", StringComparison.Ordinal) + 1);
                    dateStr = dateStr.Substring(0, dateStr.IndexOf("<", StringComparison.Ordinal));
                    return DateTime.Parse(dateStr);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "GetArticleDate in Zeit failed", this, ex);
            }
            return null;
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                articleModel.Content.Clear();

                var article = await DownloadAsync(articleModel);
                var date = GetArticleDate(article);
                if (date != null)
                    articleModel.PublishDateTime = date.Value;

                article = article.Substring(article.IndexOf(">", StringComparison.Ordinal) + 1).Trim();
                bool imgGalWarning = article.StartsWith("<gallery");

                article = RepairArticleXml(article);
                if (article == null)
                    return false;

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(article);

                var content = doc.DocumentNode
                    .Descendants("p");
                var html = content.Aggregate("", (current, htmlNode) => current + htmlNode.OuterHtml);
                var para = HtmlConverter.CreateOnce(articleModel.Feed.Source.PublicBaseUrl).HtmlToParagraph(html);
                if (para.Count > 0)
                    articleModel.Content.Add(new TextContentModel()
                    {
                        Content = para
                    });

                if (imgGalWarning)
                    articleModel.Content.Add(TextHelper.TextToTextModel("Bildergalerien werden leider noch nicht unterstützt."));

                return true;
            });
        }

        protected override async Task<string> DownloadAsync(Uri url)
        {
            using (var client = new HttpClient())
            {
                var stream = await client.GetStreamAsync(url);
                using (var reader = new StreamReader(stream, Encoding.GetEncoding("iso-8859-1")))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
