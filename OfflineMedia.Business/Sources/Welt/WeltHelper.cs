using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using OfflineMedia.Business.Sources.Welt.Models;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources.Welt
{
    public class WeltHelper : IMediaSourceHelper
    {
        public async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                feed = HtmlHelper.RemoveXmlLvl(feed);

                feed = feed.Replace(" & ", " &amp; ");

                var channel = XmlHelper.Deserialize<Channel>(feed);

                if (channel == null)
                    LogHelper.Instance.Log(LogLevel.Error, this, "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation");
                else
                {
                    foreach (var children in channel.Item)
                    {
                        var article = await FeedToArticleModel(children, fcm);
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

        private async Task<ArticleModel> FeedToArticleModel(Item children, FeedConfigurationModel fcm)
        {
            if (children == null)
                return null;

            try
            {
                var title = children.Title.Substring(0, children.Title.IndexOf(":", StringComparison.Ordinal));
                var subTitle = children.Title.Substring(children.Title.IndexOf(":", StringComparison.Ordinal) + 2);
                var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();
                var a = new ArticleModel
                {
                    Title = title,
                    SubTitle = subTitle,
                    Teaser = children.Description,
                    PublicationTime = DateTime.Parse(children.PubDate),
                    PublicUri = new Uri(children.Guid),
                    LogicUri = new Uri(children.Link),
                    Themes = new List<ThemeModel>
                    {
                        await repo.GetThemeModelFor(children.Category)
                    }
                };

                if (children.Enclosure != null)
                {
                    a.LeadImage = new ImageModel { Url = new Uri(children.Enclosure.Url) };
                }

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.FeedToArticleModel failed", ex);
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
                await Task.Run(() =>
                {
                    var head = XmlHelper.GetSingleNode(article, "body.head");
                    var author = XmlHelper.GetSingleNode(head, "byline");
                    if (author != null)
                    {
                        am.Author = author.Substring(("<byline>").Length, author.Length - ("<byline>").Length * 2 - 1);
                    }


                    var body = XmlHelper.GetSingleNode(article, "body.content");
                    body = body.Replace("<hl2>", "<h2>");
                    body = body.Replace("</hl2>", "</h2>");
                    body = XmlHelper.RemoveNodes(body, "media", "block class=\"related-links\"");

                    am.Content = new List<ContentModel>
                        {
                            new ContentModel {Html = body, ContentType = ContentType.Html}
                        };
                });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
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
    }
}
