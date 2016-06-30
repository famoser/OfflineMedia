using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Helpers.Text;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.RelationModels;
using OfflineMedia.Business.Newspapers.Welt.Models;
using OfflineMedia.Business.Repositories.Interfaces;

namespace OfflineMedia.Business.Newspapers.Welt
{
    public class WeltHelper : BaseMediaSourceHelper
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

                feed = feed.Replace(" & ", " &amp; ");

                var channel = XmlHelper.Deserialize<Channel>(feed);

                if (channel == null)
                    LogHelper.Instance.Log(LogLevel.Error, "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation", this);
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
                LogHelper.Instance.Log(LogLevel.Error, "ZwanzigMinHelper.EvaluateFeed failed", this, ex);
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
                LogHelper.Instance.Log(LogLevel.Error, "ZwanzigMinHelper.FeedToArticleModel failed", this, ex);
                return null;
            }
        }

        public override async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
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
    }
}
