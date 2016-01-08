using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources.Bild.Models.Article;
using OfflineMedia.Business.Sources.Bild.Models.Feed;
using OfflineMedia.Business.Sources.ZwanzigMin.Models;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Helpers;
using ChildNode = OfflineMedia.Business.Sources.Bild.Models.Feed.ChildNode;

namespace OfflineMedia.Business.Sources.Bild
{
    public class BildHelper : BaseMediaSourceHelper
    {
        public override async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm)
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                var rootObj = JsonConvert.DeserializeObject<FeedRoot>(feed);

                if (rootObj == null)
                    LogHelper.Instance.Log(LogLevel.Error, this, "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation");
                else
                {
                    foreach (var children in rootObj.__childNodes__)
                    {
                        if (children.__childNodes__ != null)
                        {
                            foreach (var childNode in children.__childNodes__.Where(a => a.targetType == "article"))
                            {
                                var article = await FeedToArticleModel(childNode, fcm);
                                if (article != null)
                                    articlelist.Add(article);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ZwanzigMinHelper.EvaluateFeed failed", ex);
            }
            return articlelist;
        }

        private async Task<ArticleModel> FeedToArticleModel(ChildNode item, FeedConfigurationModel fcm)
        {
            if (item == null || item.klub != null)
                return null;

            try
            {
                var a = new ArticleModel
                {
                    Title = item.title,
                    SubTitle = item.kicker,
                    Teaser = item.text,
                    LogicUri = new Uri(item.linkURL),
                    PublicUri = new Uri(fcm.SourceConfiguration.PublicBaseUrl + item.docpath),
                };

                if (item.teaserImageURL != null)
                {
                    var url = item.teaserImageURL;
                    if (url.Contains("w=320"))
                        url = url.Replace("w=320", "w=400");
                    a.LeadImage = new ImageModel() { Url = new Uri(url) };
                }

                return a;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BildHelper.FeedToArticleModel failed", ex);
            }
            return null;
        }

        public override async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            try
            {
                await Task.Run(() =>
                {
                    var rootObj = JsonConvert.DeserializeObject<ArticleRoot>(article);

                    if (rootObj == null)
                        LogHelper.Instance.Log(LogLevel.Error, this,
                            "BildHelper.EvaluateFeed failed: rootObj is null after deserialisation");
                    else
                    {
                        am.Content = new List<ContentModel>();
                        foreach (var text in rootObj.text)
                        {
                            if (text.__nodeType__ == "CDATA")
                            {
                                if (!text.CDATA.Contains("PS: Sind Sie bei Facebook?"))
                                    am.Content.Add(new ContentModel { Html = text.CDATA, ContentType = ContentType.Html });
                            }
                        }
                        am.PublicationTime = DateTime.Parse(rootObj.pubDate);
                        am.Author = rootObj.author;
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
                return new Tuple<bool, ArticleModel>(false, am);
            }
            return new Tuple<bool, ArticleModel>(true, am);
        }

        public new bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Content = evaluatedArticle.Content;
            original.PublicationTime = evaluatedArticle.PublicationTime;
            return true;
        }
    }
}
