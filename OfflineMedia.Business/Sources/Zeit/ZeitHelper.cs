using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Ioc;
using HtmlAgilityPack;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources.Zeit.Models;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources.Zeit
{
    public class ZeitHelper : IMediaSourceHelper
    {
        public async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                if (!RepairFeedXml(ref feed))
                    return articlelist;

                var serializer = new XmlSerializer(typeof(Feed));
                TextReader reader = new StringReader(feed);

                try
                {
                    var zeitFeed = (Feed)serializer.Deserialize(reader);
                    if (zeitFeed == null)
                        LogHelper.Instance.Log(LogLevel.Error, this, "ZeitHelper.EvaluateFeed failed: Feed is null after deserialisation");
                    else
                    {
                        foreach (var item in zeitFeed.Cluster)
                        {
                            foreach (var region in item.Region)
                            {
                                foreach (var feedArticle in region.Container)
                                {

                                    var article = await FeedToArticleModel(feedArticle, fcm);
                                    if (article != null)
                                        articlelist.Add(article);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("There is an error in XML document (1, "))
                    {
                        //example: "There is an error in XML document (1, 46013)."
                        var ms = ex.Message.Substring("There is an error in XML document (1, ".Length);
                        var index = Convert.ToInt32(ms.Substring(0, ms.Length - 2));
                        feed = feed.Insert(index, "\n\n\n");
                        if (index > 1000)
                        {
                            var subtr = feed.Substring(index - 1000);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZeitHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        private async Task<ArticleModel> FeedToArticleModel(Container feedArticle, FeedConfigurationModel fcm)
        {
            if (feedArticle?.Block == null) return null;

            //block articles from other domains and videos
            if (!feedArticle.Block.Href.Contains("xml.zeit.de/") ||
                feedArticle.Block.Href.Contains("zeit.de/video/"))
                return null;


            try
            {
                var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                var link = "http://" + feedArticle.Block.Href.Trim().Substring(2);
                var pubLink = link.Replace("xml.zeit.de", "zeit.de");
                var a = new ArticleModel
                {
                    Title = feedArticle.Block.Title,
                    SubTitle = feedArticle.Block.Supertitle,
                    Teaser = feedArticle.Block.Description ?? feedArticle.Block.Text,
                    PublicationTime = DateTime.Now,
                    PublicUri = new Uri(pubLink),
                    LogicUri = new Uri(link),
                    Themes = new List<ThemeModel>()
                    {
                        await repo.GetThemeModelFor(feedArticle.Block.Ressort)
                    }
                };

                if (feedArticle.Block.Image != null && feedArticle.Block.Image.Baseid != null && !string.IsNullOrEmpty(feedArticle.Block.Image.Type))
                {
                    var url = feedArticle.Block.Image.Baseid.Trim();
                    if (url.Contains("//xml.zeit.de"))
                    {
                        a.LeadImage = new ImageModel() { Url = new Uri("http://" + url.Replace("//xml.zeit.de", "zeit.de") + "cinema__940x403") };
                    }
                }

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZeitHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public bool NeedsToEvaluateArticle()
        {
            return true;
        }

        public async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            try
            {
                var date = GetArticleDate(article);
                if (date != null)
                    am.PublicationTime = date.Value;

                article = article.Substring(article.IndexOf(">", StringComparison.Ordinal) + 1).Trim();
                if (article.StartsWith("<gallery"))
                {
                    am.Content = new List<ContentModel> { new ContentModel { Html = "Bildergalerien werden leider nicht unterstützt.", ContentType = ContentType.Html } };
                    return new Tuple<bool, ArticleModel>(true, am);
                }

                if (!RepairArticleXml(ref article))
                    return new Tuple<bool, ArticleModel>(false, am);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(article);

                var content = doc.DocumentNode
                    .Descendants("p");
                var html = content.Aggregate("", (current, htmlNode) => current + htmlNode.OuterHtml);
                am.Content = new List<ContentModel> { new ContentModel() { Html = html, ContentType = ContentType.Html } };
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "EvaluateArticle failed for Zeit", ex);
                return new Tuple<bool, ArticleModel>(false, am);
            }
            return new Tuple<bool, ArticleModel>(true, am);
        }

        public bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Author = evaluatedArticle.Author;
            original.Content = evaluatedArticle.Content;
            return true;
        }

        public List<string> GetKeywords(ArticleModel articleModel)
        {
            var part1 = TextHelper.Instance.GetImportantWords(articleModel.Title);
            var part2 = TextHelper.Instance.GetImportantWords(articleModel.SubTitle);

            return TextHelper.Instance.FusionLists(part1, part2);
        }

        public bool RepairFeedXml(ref string xml)
        {
            try
            {
                var hrefPattern = "[^\"]";
                xml = XmlHelper.GetNodes(xml, "cluster").Aggregate((c, s) => c + s);

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
                LogHelper.Instance.Log(LogLevel.Error, this, "RepairFeedXml in Zeit failed", ex);
                return false;
            }
        }

        public bool RepairArticleXml(ref string xml)
        {
            xml = XmlHelper.GetSingleNode(xml, "body");
            if (xml != null)
            {
                xml = XmlHelper.GetSingleNode(xml, "division");
                if (xml != null)
                {
                    xml = xml.Replace("intertitle", "h2");
                    return true;
                }
            }
            return false;
        }

        public DateTime? GetArticleDate(string xml)
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
                LogHelper.Instance.Log(LogLevel.Error, this, "GetArticleDate in Zeit failed", ex);
            }
            return null;
        }
    }
}
