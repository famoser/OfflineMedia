using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.Business.Repositories.Mocks
{
    public class ThemeRepositoryMock : IThemeRepository
    {
        public async Task AddThemeToArticleAsync(ArticleModel article, string theme)
        {
        }

        public async Task AddRelatedThemesArticlesAsync(ArticleModel article)
        {

        }

        public async Task LoadArticleThemesAsync(ArticleModel am)
        {
        }
    }
}
