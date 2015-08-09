using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Models.NewsModel.NMModels;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;
using OfflineMediaV3.Data.Entities;

namespace OfflineMediaV3.Business.Framework.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        public async Task<bool> SetThemesByArticle(int articleId, List<int> themeIds, IDataService dataService)
        {
            var relations = await GetRelationsByArticleId(articleId, dataService);

            var repo = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(dataService);
            foreach (var themeArticleRelationModel in relations)
            {
                if (themeIds.Contains(themeArticleRelationModel.ThemeId))
                    themeIds.Remove(themeArticleRelationModel.ThemeId);
                else
                {
                    if (!await repo.Delete(themeArticleRelationModel))
                        return false;
                }
            }

            foreach (var themeId in themeIds)
            {
                var model = new ThemeArticleRelationModel()
                {
                    ArticleId = articleId,
                    ThemeId = themeId
                };

                if ((await repo.AddOrUpdate(model)) == -1)
                    return false;
            }
            return true;
        }

        public async Task<List<ThemeModel>> GetThemesByArticleId(int articleId, IDataService dataService)
        {
            var relations = await GetRelationsByArticleId(articleId, dataService);

            var repo2 = new GenericRepository<ThemeModel, ThemeEntity>(dataService);
            var res = new List<ThemeModel>();
            foreach (var themeArticleRelationModel in relations)
            {
                var item = await repo2.GetById(themeArticleRelationModel.ThemeId);
                if (item != null)
                    res.Add(item);
            }
            return res;
        }

        private async Task<List<ThemeArticleRelationModel>> GetRelationsByArticleId(int articleId, IDataService dataService)
        {
            var repo = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(dataService);
            return await repo.GetByCondition(d => d.ArticleId == articleId);
        }
    }
}
