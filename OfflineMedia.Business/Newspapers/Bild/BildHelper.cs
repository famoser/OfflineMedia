using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers.Text;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Newspapers.Bild.Models.Article;
using OfflineMedia.Business.Newspapers.Bild.Models.Feed;
using OfflineMedia.Business.Repositories.Interfaces;
using ChildNode = OfflineMedia.Business.Newspapers.Bild.Models.Feed.ChildNode;

namespace OfflineMedia.Business.Newspapers.Bild
{
    public class BildHelper : BaseMediaSourceHelper
    {
        private ArticleModel FeedToArticleModel(ChildNode item, FeedModel fcm)
        {
            if (item == null || item.klub != null)
                return null;

            return ExecuteSafe(() =>
            {

                var a = ConstructArticleModel(fcm);

                a.Title = item.title;
                a.SubTitle = item.kicker;
                a.Teaser = item.text;
                a.LogicUri = item.linkURL;
                a.PublicUri = fcm.Source.PublicBaseUrl + item.docpath;

                if (item.teaserImageURL != null)
                {
                    var url = item.teaserImageURL;
                    if (url.Contains("w=320"))
                        url = url.Replace("w=320", "w=400");
                    a.LeadImage = new ImageContentModel() { Url = url };
                }

                return a;

            });
        }

        public BildHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var feed = await DownloadAsync(feedModel);
                var articlelist = new List<ArticleModel>();
                if (feed == null) return articlelist;

                var rootObj = JsonConvert.DeserializeObject<FeedRoot>(feed);

                if (rootObj == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation", this);
                else
                {
                    foreach (var children in rootObj.__childNodes__)
                    {
                        if (children.__childNodes__ != null)
                        {
                            foreach (var childNode in children.__childNodes__.Where(a => a.targetType == "article"))
                            {
                                var article = FeedToArticleModel(childNode, feedModel);
                                if (article != null)
                                    articlelist.Add(article);
                            }
                        }
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

                var rootObj = JsonConvert.DeserializeObject<ArticleRoot>(article);

                if (rootObj == null)
                    LogHelper.Instance.Log(LogLevel.Error,
                        "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation", this);
                else
                {
                    foreach (var text in rootObj.text)
                    {
                        if (text.__nodeType__ == "CDATA")
                        {
                            if (!text.CDATA.Contains("PS: Sind Sie bei Facebook?"))
                                articleModel.Content.Add(new
                                    TextContentModel()
                                {
                                    Content = HtmlConverter.HtmlToParagraph(text.CDATA)
                                });
                        }
                    }
                    articleModel.PublishDateTime = DateTime.Parse(rootObj.pubDate);
                    articleModel.Author = rootObj.author;
                }
                return true;
            });
        }
    }
}
