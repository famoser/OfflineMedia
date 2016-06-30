using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        ObservableCollection<SourceModel> GetActiveSources();
        ObservableCollection<SourceModel> GetAllSources();

        Task<bool> LoadFullArticleAsync(ArticleModel am);
        Task<bool> LoadFullFeedAsync(FeedModel fm);

        Task<bool> ActualizeAllArticlesAsync();
        Task<bool> ActualizeArticleAsync(ArticleModel am);

        Task<bool> SetArticleFavoriteStateAsync(ArticleModel am, bool isFavorite);
        Task<bool> MarkArticleAsReadAsync(ArticleModel am);

        /// <summary>
        /// Get article explaining what this application is
        /// </summary>
        /// <returns></returns>
        ArticleModel GetInfoArticle();

        /// <summary>
        /// for design view
        /// </summary>
        /// <returns></returns>
        ObservableCollection<SourceModel> GetSampleSources();
    }
}
