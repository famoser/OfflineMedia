using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.RelationModels;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Data;
#pragma warning disable 1998

namespace OfflineMedia.Fakes
{
    class FakeThemeRepository : IThemeRepository
    {
        public async Task<bool> SaveChanges(IDataService dataService = null)
        {
            return true;
        }

        public async Task<bool> Initialize()
        {
            return true;
        }

        public async Task<bool> SetThemesByArticle(int articleId, List<int> themeIds)
        {
            return true;
        }

        public async Task<Tuple<List<ThemeArticleRelationModel>, List<ThemeArticleRelationModel>>> SetChangesByArticle(int articleId, List<int> themeIds)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ThemeModel>> GetThemesByArticleId(int articleId)
        {
            var res = new List<ThemeModel> {new ThemeModel {Name = "Fake Theme"}};
            return res;
        }

        public async Task<ThemeModel> GetThemeModelFor(string theme)
        {
            return new ThemeModel() {Name = theme};
        }

        public async Task<List<ThemeModel>> GetThemeModelsFor(string[] theme)
        {
            return theme.Select(s => new ThemeModel() {Name = s}).ToList();
        }
    }
}
