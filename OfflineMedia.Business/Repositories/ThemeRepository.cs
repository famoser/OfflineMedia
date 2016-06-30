using System;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Repositories.Interfaces;

namespace OfflineMedia.Business.Repositories
{
    public class ThemeRepository : IThemeRepository
    {
        public Task<bool> AddThemeToArticleAsync(ArticleModel article, string theme)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddRelatedThemesArticlesAsync(ArticleModel article)
        {
            throw new NotImplementedException();
        }
    }
}
