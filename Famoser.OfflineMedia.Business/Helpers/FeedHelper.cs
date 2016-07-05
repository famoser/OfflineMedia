using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Data.Entities.Database.Relations;
using Famoser.SqliteWrapper.Services.Interfaces;

namespace Famoser.OfflineMedia.Business.Helpers
{
    public class FeedHelper
    {
        public static async Task SaveFeed(FeedModel model, List<ArticleModel> newArticles, ISqliteService service)
        {
            var stringGuid = model.Guid.ToString();
            var feedEntries = new Stack<FeedArticleRelationEntity>(await service.GetByCondition<FeedArticleRelationEntity>(d => d.FeedGuid == stringGuid, null, false, 0, 0));
            var oldArticles = new List<ArticleModel>(model.AllArticles);
            model.AllArticles.Clear();

            foreach (var articleModel in newArticles)
            {
                var oldOne = oldArticles.FirstOrDefault(s => s.PublicUri == articleModel.PublicUri);
                if (oldOne != null)
                {
                    model.AllArticles.Add(oldOne);
                }
                else
                {
                    model.AllArticles.Add(articleModel);
                    await ArticleHelper.SaveArticle(articleModel, service);
                    await ArticleHelper.SaveArticleLeadImage(articleModel, service, true);
                    await ArticleHelper.SaveArticleContent(articleModel, service, true);
                }
            }

            for (int i = 0; i < model.AllArticles.Count; i++)
            {
                if (feedEntries.Count > 0)
                {
                    var entry = feedEntries.Pop();
                    entry.ArticleId = model.AllArticles[i].GetId();
                    entry.Index = i;
                    await service.Update(entry);
                }
                else
                {
                    var entry = new FeedArticleRelationEntity()
                    {
                        ArticleId = model.AllArticles[i].GetId(),
                        FeedGuid = model.Guid.ToString(),
                        Index = i
                    };
                    await service.Add(entry);
                }
            }

            while (feedEntries.Count > 0)
            {
                var entry = feedEntries.Pop();
                await service.DeleteById<FeedArticleRelationEntity>(entry.Id);
            }
        }
    }
}
