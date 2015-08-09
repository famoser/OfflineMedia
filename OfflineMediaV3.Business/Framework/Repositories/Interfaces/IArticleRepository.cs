using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.Business.Framework.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        ObservableCollection<SourceModel> GetSampleArticles();

        ArticleModel GetInfoArticle();

        Task<bool> UpdateArticle(ArticleModel article);

        Task<ObservableCollection<SourceModel>> GetSources();

        Task<ObservableCollection<ArticleModel>> GetArticleByFeed(Guid feedId, int max = 0);

        Task ActualizeArticles(IProgressService progressService);

        Task<ArticleModel> ActualizeArticle(ArticleModel model);

        Task<ArticleModel> GetCompleteArticle(int articleId, bool automaticallyActualize = false);
    }
}
