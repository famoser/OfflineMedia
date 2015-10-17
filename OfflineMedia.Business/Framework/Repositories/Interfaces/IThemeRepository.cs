using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.NMModels;
using OfflineMedia.Data;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface IThemeRepository
    {
        Task<bool> SaveChanges(IDataService dataService = null);
        Task<bool> Initialize();

        Task<bool> SetThemesByArticle(int articleId, List<int> themeIds);
        Task<Tuple<List<ThemeArticleRelationModel>, List<ThemeArticleRelationModel>>> SetChangesByArticle(int articleId, List<int> themeIds);

        Task<List<ThemeModel>> GetThemesByArticleId(int articleId);
        Task<ThemeModel> GetThemeModelFor(string theme);
        Task<List<ThemeModel>> GetThemeModelsFor(string[] theme);
    }
}
