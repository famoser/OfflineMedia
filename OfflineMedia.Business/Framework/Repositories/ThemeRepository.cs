using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.NMModels;
using OfflineMedia.Data;
using OfflineMedia.Data.Entities;
using OfflineMedia.Data.Entities.Database;
using OfflineMedia.Data.Entities.Database.Relations;
using OfflineMedia.Data.Repository.EqualityComparer;

namespace OfflineMedia.Business.Framework.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        private List<ThemeModel> _themes = new List<ThemeModel>();
        private List<ThemeModel> _newthemes = new List<ThemeModel>();
        private List<ThemeArticleRelationModel> _relations = new List<ThemeArticleRelationModel>();
        private List<ThemeArticleRelationModel> _newRelations = new List<ThemeArticleRelationModel>();
        private List<ThemeArticleRelationModel> _deleteRelations = new List<ThemeArticleRelationModel>();
        public async Task<bool> SaveChanges(IDataService dataService = null)
        {
            if (dataService == null)
            {
                using (var unitOfWork = new UnitOfWork(true))
                {
                    return await SaveChangesInternal(await unitOfWork.GetDataService());
                }
            }
            return await SaveChangesInternal(dataService);
        }

        public async Task<bool> SaveChangesInternal(IDataService dataService)
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo1 = new GenericRepository<ThemeModel, ThemeEntity>(await unitOfWork.GetDataService());

                await repo1.AddAll(_newthemes);
                _newthemes = new List<ThemeModel>();


                var repo2 = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(await unitOfWork.GetDataService());

                await repo2.AddAll(_newRelations);
                _newRelations = new List<ThemeArticleRelationModel>();

                await repo2.DeleteAll(_deleteRelations);
                _deleteRelations = new List<ThemeArticleRelationModel>();
            }
            return true;
        }

        private bool _isInitialized;
        public async Task<bool> Initialize()
        {
            using (var unitOfWork = new UnitOfWork(true))
            {
                var repo1 = new GenericRepository<ThemeModel, ThemeEntity>(await unitOfWork.GetDataService());
                _themes = await repo1.GetByCondition(d => true);
                var repo2 = new GenericRepository<ThemeArticleRelationModel, ThemeArticleRelations>(await unitOfWork.GetDataService());
                _relations = await repo2.GetByCondition(d => true);
            }
            _isInitialized = true;

            return true;
        }

        public async Task<bool> SetThemesByArticle(int articleId, List<int> themeIds)
        {
            var res = await SetChangesByArticle(articleId, themeIds);
            _deleteRelations.AddRange(res.Item1);
            _newRelations.AddRange(res.Item2);

            foreach (var themeArticleRelationModel in res.Item1)
            {
                _relations.Remove(themeArticleRelationModel);
            }
            _relations.AddRange(res.Item2);

            return true;
        }

        public async Task<Tuple<List<ThemeArticleRelationModel>, List<ThemeArticleRelationModel>>> SetChangesByArticle(int articleId, List<int> themeIds)
        {
            if (!_isInitialized)
                await Initialize();

            var oldones = new List<ThemeArticleRelationModel>();
            var newones = new List<ThemeArticleRelationModel>();

            themeIds = themeIds.Distinct(new IntEqualityComparer()).ToList();
            var relations = _relations.Where(d => d.ArticleId == articleId);

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

        public async Task<List<ThemeModel>> GetThemesByArticleId(int articleId)
        {
            if (!_isInitialized)
                await Initialize();

            var relations = _relations.Where(d => d.ArticleId == articleId);

            var res = new List<ThemeModel>();
            foreach (var themeArticleRelationModel in relations)
            {
                var item = _themes.FirstOrDefault(d => d.Id == themeArticleRelationModel.ThemeId);
                if (item != null)
                    res.Add(item);
            }
            return res;
        }

        public async Task<ThemeModel> GetThemeModelFor(string theme)
        {
            if (!_isInitialized)
                await Initialize();

            var themeModel = _themes.FirstOrDefault(t => t.Name == theme);
            if (themeModel == null)
            {
                themeModel = new ThemeModel()
                {
                    Name = theme
                };
                _newthemes.Add(themeModel);
                _themes.Add(themeModel);
            }
            return themeModel;
        }

        public async Task<List<ThemeModel>> GetThemeModelsFor(string[] theme)
        {
            if (!_isInitialized)
                await Initialize();

            var list = new List<ThemeModel>();
            foreach (var s in theme)
            {
                list.Add(await GetThemeModelFor(s));
            }
            return list;
        }
    }
}
