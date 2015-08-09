using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMediaV3.Business.Framework.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        ObservableCollection<SourceModel> GetSampleArticles();

        ArticleModel GetInfoArticle();

        Task UpdateArticle(ArticleModel article);

        Task<ObservableCollection<SourceModel>> GetSources();

        Task<ObservableCollection<ArticleModel>> GetArticlesByFeed(Guid feedId, int max = 0);

        Task ActualizeArticles(IProgressService progressService);

        Task<ArticleModel> ActualizeArticle(ArticleModel model);

        Task<ArticleModel> GetCompleteArticle(int articleId);
    }
}
