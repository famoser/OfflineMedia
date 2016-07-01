using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers.Text;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Newspapers.Blick.Models;
using OfflineMedia.Business.Newspapers.Welt.Models;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Data.Entities.Database.Contents;

namespace OfflineMedia.Business.Newspapers.Welt
{
    public class WeltHelper : BaseMediaSourceHelper
    {
        public WeltHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        private ArticleModel FeedToArticleModel(Item children, FeedModel fcm)
        {
            if (children == null)
                return null;

            return ExecuteSafe(() =>
            {
                var title = children.Title.Substring(0, children.Title.IndexOf(":", StringComparison.Ordinal));
                var subTitle = children.Title.Substring(children.Title.IndexOf(":", StringComparison.Ordinal) + 2);

                var a = ConstructArticleModel(fcm);
                a.Title = title;
                a.SubTitle = subTitle;
                a.Teaser = children.Description;
                a.PublishDateTime = DateTime.Parse(children.PubDate);
                a.PublicUri = children.Guid;
                a.LogicUri = children.Link;

                if (children.Enclosure != null)
                    a.LeadImage = new ImageContentModel() { Url = children.Enclosure.Url };

                return a;
            });
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var articlelist = new List<ArticleModel>();
                var feed = await DownloadAsync(feedModel);
                if (feed == null) return articlelist;

                feed = feed.Substring(feed.IndexOf(">", StringComparison.Ordinal));
                feed = feed.Substring(feed.IndexOf("<", StringComparison.Ordinal));
                feed = HtmlHelper.RemoveXmlLvl(feed);

                feed = feed.Replace(" & ", " &amp; ");

                var channel = XmlHelper.Deserialize<Channel>(feed);

                if (channel == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation", this);
                else
                {
                    foreach (var children in channel.Item)
                    {
                        var article = FeedToArticleModel(children, feedModel);
                        if (article != null)
                            articlelist.Add(article);
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
               var head = XmlHelper.GetSingleNode(article, "body.head");
               var author = XmlHelper.GetSingleNode(head, "byline");
               if (author != null)
               {
                   articleModel.Author = author.Substring(("<byline>").Length, author.Length - ("<byline>").Length * 2 - 1);
               }

               var body = XmlHelper.GetSingleNode(article, "body.content");
               body = body.Replace("<hl2>", "<h2>");
               body = body.Replace("</hl2>", "</h2>");
               body = XmlHelper.RemoveNodes(body, "media", "block class=\"related-links\"");

               articleModel.Content.Add(new TextContentModel()
               {
                   Content = HtmlConverter.HtmlToParagraph(body)
               });
               
               await AddThemesAsync(a, new[] { children.Category });
               return true;
           });
        }

    }
}