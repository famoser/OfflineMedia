using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

namespace Famoser.OfflineMedia.View.Mocks
{
    class ArticleRepositoryMock : IArticleRepository
    {
        public ObservableCollection<SourceModel> GetActiveSources()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<SourceModel> GetAllSources()
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoadFullArticleAsync(ArticleModel am)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoadFullFeedAsync(FeedModel fm)
        {
            throw new NotImplementedException();
        }

        public Task ActualizeAllArticlesAsync()
        {
            throw new NotImplementedException();
        }

        public Task ActualizeArticleAsync(ArticleModel am)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetArticleFavoriteStateAsync(ArticleModel am, bool isFavorite)
        {
            throw new NotImplementedException();
        }

        public Task<bool> MarkArticleAsReadAsync(ArticleModel am)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetFeedActiveStateAsync(FeedModel feedModel, bool isActive)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetSourceActiveStateAsync(SourceModel sourceModel, bool isActive)
        {
            throw new NotImplementedException();
        }

        public ArticleModel GetInfoArticle()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<SourceModel> GetSampleSources()
        {
            throw new NotImplementedException();
        }
    }
}
