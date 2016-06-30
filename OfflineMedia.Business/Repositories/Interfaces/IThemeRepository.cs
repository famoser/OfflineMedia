using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Repositories.Interfaces
{
    public interface IThemeRepository
    {
        Task<bool> AddThemeToArticleAsync(ArticleModel article, string theme);
        Task<bool> AddRelatedThemesArticlesAsync(ArticleModel article);
    }
}
