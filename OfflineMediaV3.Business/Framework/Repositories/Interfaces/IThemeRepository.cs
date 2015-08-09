using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;

namespace OfflineMediaV3.Business.Framework.Repositories.Interfaces
{
    public interface IThemeRepository
    {
        Task<bool> SetThemesByArticle(int articleId, List<int> themeIds, IDataService dataService);
        Task<List<ThemeModel>> GetThemesByArticleId(int articleId, IDataService dataService);
    }
}
