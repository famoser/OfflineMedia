using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace Famoser.OfflineMedia.Business.Repositories.Interfaces
{
    public interface IThemeRepository
    {
        Task AddThemeToArticleAsync(ArticleModel article, string theme);
        Task AddRelatedThemesArticlesAsync(ArticleModel article);
        Task LoadArticleThemesAsync(ArticleModel am);
    }
}
