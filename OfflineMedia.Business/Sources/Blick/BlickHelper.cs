using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources.Blick.Models;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Singleton;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources.Blick
{
    public class BlickHelper :BaseMediaSourceHelper
    {
#pragma warning disable 1998
        public override async Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scm, FeedConfigurationModel fcm)
#pragma warning restore 1998
        {
            var articlelist = new List<ArticleModel>();
            if (feed == null) return articlelist;

            try
            {
                var f = JsonConvert.DeserializeObject<feed[]>(feed);
                feed articlefeed = f.FirstOrDefault(i => i.type == "teaser");
                if (articlefeed != null)
                {
                    articlelist.AddRange(articlefeed.items.Where(i => i.type == "teaser")
                        .Select(FeedModelToArticleModel)
                        .Where(am => am != null));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.EvaluateFeed deserialization failed", ex);
            }
            return articlelist;
        }

        private ArticleModel FeedModelToArticleModel(feeditem item)
        {
            try
            {
                var am = new ArticleModel
                {
                    Title = item.title,
                    LogicUri = new Uri(item.targetUrl),
                    PublicUri = new Uri(item.targetUrl.Substring(0, item.targetUrl.IndexOf(".json"))),
                    PublicationTime = item.publicationDate,
                    SubTitle = item.catchword,
                    Teaser = item.lead,
                    LeadImage = new ImageModel() { Url = new Uri(item.img.src) }
                };
                return am;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.FeedModelToArticleModel failed", ex);
                return null;
            }
        }

        public override async Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            if (article == null) return new Tuple<bool, ArticleModel>(false, am);

            try
            {
                var a = JsonConvert.DeserializeObject<articlefeeditem[]>(article);

                return await WriteProperties(a, am);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.EvaluateArticle failed", ex);
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }

        public new bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Content = evaluatedArticle.Content;
            original.Author = evaluatedArticle.Author;
            original.Themes = evaluatedArticle.Themes;

            return true;
        }

        public async Task<Tuple<bool, ArticleModel>> WriteProperties(articlefeeditem[] na, ArticleModel am)
        {
            if (na == null) return new Tuple<bool, ArticleModel>(false, am);

            try
            {
                articlefeeditem category = na.FirstOrDefault(a => a.type == "metadata");
                if (category != null)
                {
                    var repo = SimpleIoc.Default.GetInstance<IThemeRepository>();

                    am.Themes = new List<ThemeModel>
                    {
                        await repo.GetThemeModelFor(category.section),
                        await repo.GetThemeModelFor(am.FeedConfiguration.Name)
                    };

                    var auth = category.author as string;
                    if (auth != null)
                        am.Author = auth;
                }

                articlefeeditem body = na.FirstOrDefault(a => a.type == "body");
                if (body != null)
                {
                    var htmlbody = body.items.Where(b => b.type == "text").ToList();
                    am.Content = new List<ContentModel>();

                    for (int i = 0; i < htmlbody.Count(); i++)
                    {
                        am.Content.Add(new ContentModel() { Html = htmlbody[i].txt, ContentType = ContentType.Html });
                    }
                }

                articlefeeditem headline = na.FirstOrDefault(a => a.type == "headline");
                if (headline != null && headline.author != null && headline.author.GetType() == typeof(articlefeeditem))
                {
                    articlefeeditem author = headline.author as articlefeeditem;
                    if (author != null)
                        am.Author = author.firstName;
                }

                return new Tuple<bool, ArticleModel>(true, am);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "BlickHelper.ArticleToArticleModel failed", ex);
            }
            return new Tuple<bool, ArticleModel>(false, am);
        }
    }
}
