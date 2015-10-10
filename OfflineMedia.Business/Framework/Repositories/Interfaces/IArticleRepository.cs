using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        ObservableCollection<SourceModel> GetSampleArticles();

        ArticleModel GetInfoArticle();

        ArticleModel GetEmptyFeedArticle();

        Task UpdateArticle(ArticleModel article);

        Task<ObservableCollection<SourceModel>> GetSources();
        Task<SourceModel> GetFavorites();

        Task<ArticleModel> GetArticleById(int articleId);
        Task<ObservableCollection<ArticleModel>> GetArticlesByFeed(Guid feedId, int max = 0, int skip = 0);

        Task ActualizeArticles();

        Task<ArticleModel> ActualizeArticle(ArticleModel model, IMediaSourceHelper msh);

        Task<ArticleModel> GetCompleteArticle(int articleId);

        Task<ObservableCollection<ArticleModel>> GetSimilarCathegoriesArticles(ArticleModel article, int max);
        Task<ObservableCollection<ArticleModel>> GetSimilarTitlesArticles(ArticleModel article, int max);
    }
}
