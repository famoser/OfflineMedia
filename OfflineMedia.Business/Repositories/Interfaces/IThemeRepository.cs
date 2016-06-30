using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Repositories.Interfaces
{
    public interface IThemeRepository
    {
        Task AddThemeToArticleAsync(ArticleModel article, string theme);
        Task AddRelatedThemesArticlesAsync(ArticleModel article);
    }
}
