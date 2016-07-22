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
            var feedEntries = await service.GetByCondition<FeedArticleRelationEntity>(d => d.FeedGuid == stringGuid, null, false, 0, 0);
            var oldArticles = new List<ArticleModel>(model.AllArticles);

            for (int index = 0; index < newArticles.Count; index++)
            {
                var articleModel = newArticles[index];
                var oldOne = oldArticles.FirstOrDefault(s => s.PublicUri == articleModel.PublicUri);
                if (oldOne == null)
                {
                    var oldFromDatabase = feedEntries.FirstOrDefault(s => articleModel.LogicUri == s.Url);
                    if (oldFromDatabase != null)
                    {
                        model.AllArticles.Add(await ArticleHelper.LoadForFeed(oldFromDatabase.ArticleId, service));
                        feedEntries.Remove(oldFromDatabase);
                        oldFromDatabase.Index = index;
                        await service.Update(oldFromDatabase);
                    }
                    else
                    {
                        await ArticleHelper.SaveArticle(articleModel, service);
                        await ArticleHelper.SaveArticleLeadImage(articleModel, service, true);
                        await ArticleHelper.SaveArticleContent(articleModel, service, true);
                        model.AllArticles.Add(articleModel);

                        var fe = new FeedArticleRelationEntity()
                        {
                            ArticleId = articleModel.GetId(),
                            Url = articleModel.LogicUri,
                            FeedGuid = model.Guid.ToString(),
                            Index = index
                        };
                        await service.Add(fe);
                    }
                }
                else
                {
                    model.AllArticles.Add(oldOne);

                    var oldFromDatabase = feedEntries.FirstOrDefault(s => articleModel.LogicUri == s.Url);
                    if (oldFromDatabase != null)
                    {
                        oldFromDatabase.Index = index;
                        await service.Update(oldFromDatabase);
                    }
                    else
                    {
                        var fe = new FeedArticleRelationEntity()
                        {
                            ArticleId = oldOne.GetId(),
                            Url = oldOne.LogicUri,
                            FeedGuid = model.Guid.ToString(),
                            Index = index
                        };
                        await service.Add(fe);
                    }
                }
            }

            await service.DeleteAllById<FeedArticleRelationEntity>(feedEntries.Select(s => s.Id));  
        }
    }
}
