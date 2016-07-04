using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace Famoser.OfflineMedia.Business.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        ObservableCollection<SourceModel> GetActiveSources();
        ObservableCollection<SourceModel> GetAllSources();

        Task<bool> LoadFullArticleAsync(ArticleModel am);
        Task<bool> LoadFullFeedAsync(FeedModel fm);

        Task ActualizeAllArticlesAsync();
        Task ActualizeArticleAsync(ArticleModel am);

        Task<bool> SetArticleFavoriteStateAsync(ArticleModel am, bool isFavorite);
        Task<bool> MarkArticleAsReadAsync(ArticleModel am);

        Task<bool> SetFeedActiveStateAsync(FeedModel feedModel, bool isActive);
        Task<bool> SetSourceActiveStateAsync(SourceModel sourceModel, bool isActive);

        Task<bool> SwitchFeedActiveStateAsync(FeedModel feedModel);
        Task<bool> SwitchSourceActiveStateAsync(SourceModel sourceModel);

        /// <summary>
        /// Get article explaining what this application is
        /// </summary>
        /// <returns></returns>
        ArticleModel GetInfoArticle();
    }
}
