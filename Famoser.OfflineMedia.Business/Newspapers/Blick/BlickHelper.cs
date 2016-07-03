using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Helpers.Text;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Newspapers.Blick.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Newspapers.Blick
{
    public class BlickHelper : BaseMediaSourceHelper
    {

        private ArticleModel FeedModelToArticleModel(feeditem item, FeedModel feedModel)
        {
            return ExecuteSafe(() =>
            {
                var am = ConstructArticleModel(feedModel);
                am.Title = item.title;
                am.LogicUri = item.targetUrl;
                am.PublicUri = item.targetUrl.Substring(0, item.targetUrl.IndexOf(".json", StringComparison.Ordinal));
                am.PublishDateTime = item.publicationDate;
                am.SubTitle = item.catchword;
                am.Teaser = item.lead;
                am.LeadImage = new ImageContentModel() {Url = item.img.src};
                return am;
            });
        }


        public async Task<bool> WriteProperties(articlefeeditem[] na, ArticleModel am)
        {
            if (na == null)
                return false;

            articlefeeditem category = na.FirstOrDefault(a => a.type == "metadata");
            if (category != null)
            {
                await AddThemesAsync(am, new[] { category.section });

                var auth = category.author as string;
                if (auth != null)
                    am.Author = auth;
            }

            articlefeeditem body = na.FirstOrDefault(a => a.type == "body");
            if (body != null)
            {
                var htmlbody = body.items.Where(b => b.type == "text").ToList();

                for (int i = 0; i < htmlbody.Count(); i++)
                {
                    am.Content.Add(new TextContentModel()
                    {
                        Content = HtmlConverter.HtmlToParagraph(htmlbody[i].txt)
                    });
                }
            }

            articlefeeditem headline = na.FirstOrDefault(a => a.type == "headline");
            if (headline != null && headline.author != null && headline.author.GetType() == typeof(articlefeeditem))
            {
                articlefeeditem author = headline.author as articlefeeditem;
                if (author != null)
                    am.Author = author.firstName;
            }

            return true;
        }

        public BlickHelper(IThemeRepository themeRepository) : base(themeRepository)
        {
        }

        public override Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel)
        {
            return ExecuteSafe(async () =>
            {
                var feed = await DownloadAsync(feedModel);

                var articlelist = new List<ArticleModel>();
                if (feed == null) return articlelist;

                var f = JsonConvert.DeserializeObject<feed[]>(feed);
                feed articlefeed = f.FirstOrDefault(i => i.type == "teaser");
                if (articlefeed != null)
                {
                    articlelist.AddRange(articlefeed.items.Where(i => i.type == "teaser")
                        .Select(i => FeedModelToArticleModel(i, feedModel))
                        .Where(am => am != null));
                }

                return articlelist;
            });
        }

        public override Task<bool> EvaluateArticle(ArticleModel articleModel)
        {
            return ExecuteSafe(async () =>
            {
                var article = await DownloadAsync(articleModel);

                var a = JsonConvert.DeserializeObject<articlefeeditem[]>(article);

                return await WriteProperties(a, articleModel);
            });
        }
    }
}
