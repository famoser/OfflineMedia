using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

namespace Famoser.OfflineMedia.View.Mocks.Repositories
{
    class ThemeRepositoryMock : IThemeRepository
    {
        public Task AddThemeToArticleAsync(ArticleModel article, string theme)
        {
            throw new NotImplementedException();
        }

        public Task AddRelatedThemesArticlesAsync(ArticleModel article)
        {
            throw new NotImplementedException();
        }
    }
}
