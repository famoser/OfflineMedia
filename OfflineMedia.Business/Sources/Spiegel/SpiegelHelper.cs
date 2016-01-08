using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GalaSoft.MvvmLight.Ioc;
using HtmlAgilityPack;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources.Blick.Models;
using OfflineMedia.Business.Sources.Spiegel.Models;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources.Spiegel
{
    public class SpiegelHelper : BaseMediaSourceHelper
    {
        public override async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                feed = HtmlHelper.RemoveXmlLvl(feed);
                feed = feed.Replace("content:encoded", "content");
                
                var channel = XmlHelper.Deserialize<Channel>(feed);
                if (channel == null)
                    LogHelper.Instance.Log(LogLevel.Error, this, "SpiegelHelper.EvaluateFeed failed: channel is null after deserialisation");
                else
                {
                    foreach (var item in channel.Item)
                    {
                        var article = await FeedToArticleModel(item, fcm);
                        if (article != null)
                            articlelist.Add(article);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        public async Task<ArticleModel> FeedToArticleModel(Item nfa, FeedConfigurationModel fcm)
        {
            if (nfa == null || nfa.Link.Contains("/video/video"))
                return null;
            
            try
            {
                var title = nfa.Title.Substring(0, nfa.Title.IndexOf(":", StringComparison.Ordinal));
                var subTitle = nfa.Title.Substring(nfa.Title.IndexOf(":", StringComparison.Ordinal) + 2);
                var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                var link = nfa.Link;
                if (link.Contains("#ref=rss"))
                    link = link.Replace("#ref=rss","");
                var a = new ArticleModel
                {
                    Title = title,
                    SubTitle = subTitle,
                    Teaser = nfa.Description,
                    PublicationTime = DateTime.Parse(nfa.PubDate),
                    PublicUri = new Uri(nfa.Link),
                    LogicUri = new Uri(link),
                    Themes = new List<ThemeModel>
                    {
                        await repo.GetThemeModelFor(nfa.Category)
                    }
                };

                if (nfa.Enclosure != null && nfa.Enclosure.Type != null && nfa.Enclosure.Type.Contains("image"))
                {
                    var url = nfa.Enclosure.Url;
                    if (url.Contains("thumbsmall"))
                    {
                        var newurl = url.Replace("thumbsmall", "panoV9free");
                        HttpRequestMessage ms = new HttpRequestMessage(HttpMethod.Head, newurl);
                        using (var client = new HttpClient())
                        {
                            var res = await client.SendAsync(ms);
                            if (res.IsSuccessStatusCode)
                                url = newurl;
                            else
                                url = url.Replace("thumbsmall", "thumb");
                        }
                    }
                    a.LeadImage = new ImageModel { Url = new Uri(url) };
                }

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.FeedToArticleModel failed", ex);
                return null;
            }
        }

        public override async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            try
            {
                await Task.Run(() =>
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(article);

                    var articleColumn = doc.DocumentNode
                        .Descendants("div")
                        .FirstOrDefault(o => o.GetAttributeValue("id", null) != null &&
                                             o.GetAttributeValue("id", null).Contains("js-article-column"));

                    var author = articleColumn
                        .Descendants("p")
                        .FirstOrDefault();

                    if (author != null)
                    {
                        am.Author = author.InnerText;
                    }

                    var content = articleColumn
                        .Descendants("div")
                        .FirstOrDefault();

                    if (content != null)
                    {
                        var ps = content.Descendants("p");
                        var html = ps.Aggregate("", (current, htmlNode) => current + htmlNode.OuterHtml);
                        am.Content = new List<ContentModel>
                        {
                            new ContentModel {Html = html, ContentType = ContentType.Html}
                        };
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "EvaluateArticle failed for Spiegel", ex);
                return new Tuple<bool, ArticleModel>(false, am);
            }
            return new Tuple<bool, ArticleModel>(true, am);
        }
    }
}
