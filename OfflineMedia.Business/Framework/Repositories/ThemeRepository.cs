using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.NMModels;
using OfflineMedia.Common.Framework.EqualityComparer;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;

namespace OfflineMedia.Business.Framework.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        public async Task<bool> SetThemesByArticle(int articleId, List<int> themeIds, IDataService dataService)
        {
            var res = await SetChangesByArticle(articleId, themeIds, dataService);
            var repo = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(dataService);
            await repo.DeleteAll(res.Item1);
            await repo.AddAll(res.Item2);
            
            return true;
        }

        public async Task<Tuple<List<ThemeArticleRelationModel>, List<ThemeArticleRelationModel>>> SetChangesByArticle(int articleId, List<int> themeIds, IDataService dataService)
        {
            var oldones = new List<ThemeArticleRelationModel>();
            var newones = new List<ThemeArticleRelationModel>();

            themeIds = themeIds.Distinct(new IntEqualityComparer()).ToList();
            var relations = await GetRelationsByArticleId(articleId, dataService);
            
            foreach (var themeArticleRelationModel in relations)
            {
                if (themeIds.Contains(themeArticleRelationModel.ThemeId))
                    themeIds.Remove(themeArticleRelationModel.ThemeId);
                else
                {
                    oldones.Add(themeArticleRelationModel);
                }
            }

            foreach (var themeId in themeIds)
            {
                var model = new ThemeArticleRelationModel()
                {
                    ArticleId = articleId,
                    ThemeId = themeId
                };
                newones.Add(model);
            }
            return new Tuple<List<ThemeArticleRelationModel>, List<ThemeArticleRelationModel>>(oldones, newones);
        }

        public async Task<List<ThemeModel>> GetThemesByArticleId(int articleId, IDataService dataService)
        {
            if (_themes == null)
                await Initialize();

            var relations = await GetRelationsByArticleId(articleId, dataService);
            
            var res = new List<ThemeModel>();
            foreach (var themeArticleRelationModel in relations)
            {
                var item = _themes.FirstOrDefault(d => d.Id == themeArticleRelationModel.ThemeId);
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

        public async Task<ThemeModel> GetThemeModelFor(string theme)
        {
            if (_themes == null)
                await Initialize();

            var themeModel = _themes.FirstOrDefault(t => t.Name == theme);
            if (themeModel == null)
            {
                themeModel = new ThemeModel()
                {
                    Name = theme
                };
                using (var unitOfWork = new UnitOfWork(true))
                {
                    var repo = new GenericRepository<ThemeModel, ThemeEntity>(await unitOfWork.GetDataService());
                    await repo.Add(themeModel);
                }
                _themes.Add(themeModel);
            }
            return themeModel;
        }

        public async Task<List<ThemeModel>> GetThemeModelsFor(string[] theme)
        {
            if (_themes == null)
                await Initialize();

            var list = new List<ThemeModel>();
            foreach (var s in theme)
            {
                list.Add(await GetThemeModelFor(s));
            }
            return list;
        }

        private List<ThemeModel> _themes; 
        private async Task Initialize(IDataService dataService = null)
        {
            if (dataService == null)
            {
                using (var unitOfWork = new UnitOfWork(true))
                {
                    await InitializeIntern(await unitOfWork.GetDataService());
                }
            }
            else
            {
                await InitializeIntern(dataService);
            }
        }

        private async Task InitializeIntern(IDataService dataService)
        {
            var repo2 = new GenericRepository<ThemeModel, ThemeEntity>(dataService);
            _themes = await repo2.GetByCondition(d => true);
        }
    }
}
