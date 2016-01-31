using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<ObservableCollection<SourceModel>> GetSources();
        Task<SourceModel> GetFavorites();

        ArticleModel GetInfoArticle();
        ArticleModel GetEmptyFeedArticle();

        Task ActualizeArticles();
        Task<bool> ActualizeArticle(ArticleModel am);

        Task<bool> UpdateArticleState(ArticleModel am);

        Task<bool> UpdateArticleFavorite(ArticleModel am);

        Task LoadMoreArticleContent(ArticleModel am, bool content = false);

        Task<ObservableCollection<ArticleModel>> GetArticlesByFeed(Guid feedId, int max = 0, int skip = 0);

        Task<ObservableCollection<ArticleModel>> GetSimilarCathegoriesArticles(ArticleModel article, int max);
        Task<ObservableCollection<ArticleModel>> GetSimilarTitlesArticles(ArticleModel article, int max);

        /// <summary>
        /// for design view
        /// </summary>
        /// <returns></returns>
        ObservableCollection<SourceModel> GetSampleArticles();
    }
}
