using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.Welt.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

namespace Famoser.OfflineMedia.Business.Newspapers.Welt
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

                var bannedSubTitles = new[]
                {
                    "alle artikel",
                    "die wichtigsten artikel"
                };

                var bannedTitles = new[]
                {
                    "Newsblog",
                };

                var lowerSub = subTitle.ToLower();
                if (bannedSubTitles.Any(s => lowerSub.Contains(s)))
                    return null;

                if (bannedTitles.Any(s => title.Contains(s)))
                    return null;


                var a = ConstructArticleModel(fcm);
                a.Title = title;
                a.SubTitle = subTitle;
                a.Teaser = children.Description;
                a.PublishDateTime = DateTime.Parse(children.PubDate);
                a.PublicUri = children.Guid;
                a.LogicUri = children.Link;

                if (children.Enclosure != null)
                    a.LeadImage = new ImageContentModel() { Url = children.Enclosure.Url };

                a.Themes.Clear();
                a.AfterSaveFunc = () => AddThemesAsync(a, new[] { children.Category });

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
                feed = XmlHelper.RemoveXmlLvl(feed);

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
               if (head == null)
                   return false;
               var author = XmlHelper.GetSingleNode(head, "byline");
               if (author != null)
               {
                   articleModel.Author = author.Substring(("<byline>").Length, author.Length - ("<byline>").Length * 2 - 1);
               }

               if (string.IsNullOrWhiteSpace(articleModel.Author))
                   articleModel.Author = "Welt";

               var body = XmlHelper.GetSingleNode(article, "body.content");
               body = body.Replace("<hl2>", "<h2>");
               body = body.Replace("</hl2>", "</h2>");
               body = XmlHelper.RemoveNodes(body, "media", "block class=\"related-links\"");

               if (body.Contains("Roboter und künstliche Intelligenz können eine Viertelmillion neue Jobs in Deutschland schaffen, glauben "))
                   return false;

               body = body.Replace("<body.content>", "");
               body = body.Replace("</body.content>", "");

               articleModel.Content.Clear();
               articleModel.Content.Add(new TextContentModel()
               {
                   Content = HtmlConverter.CreateOnce(articleModel.Feed.Source.PublicBaseUrl).HtmlToParagraph(body)
               });

               return true;
           });
        }

    }
}