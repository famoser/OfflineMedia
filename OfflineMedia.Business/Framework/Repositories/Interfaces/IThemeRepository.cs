using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.NMModels;
using OfflineMedia.Data;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface IThemeRepository
    {
        Task<bool> AddThemeToArticleAsync(ArticleModel article, string theme);
        Task<bool> AddRelatedThemesArticlesAsync(ArticleModel article);
    }
}
