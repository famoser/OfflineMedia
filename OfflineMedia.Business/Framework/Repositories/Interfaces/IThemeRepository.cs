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
        Task<bool> SetThemesByArticle(int articleId, List<int> themeIds, IDataService dataService);
        Task<Tuple<List<ThemeArticleRelationModel>, List<ThemeArticleRelationModel>>> SetChangesByArticle(int articleId, List<int> themeIds, IDataService dataService);

        Task<List<ThemeModel>> GetThemesByArticleId(int articleId, IDataService dataService);
        Task<ThemeModel> GetThemeModelFor(string theme);
        Task<List<ThemeModel>> GetThemeModelsFor(string[] theme);
    }
}
