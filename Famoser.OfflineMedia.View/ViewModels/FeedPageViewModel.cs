using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class FeedPageViewModel : ViewModelBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IHistoryNavigationService _historyNavigationService;
        public FeedPageViewModel(IArticleRepository articleRepository, IHistoryNavigationService historyNavigationService)
        {
            _articleRepository = articleRepository;
            _historyNavigationService = historyNavigationService;

            if (IsInDesignMode)
            {
                SelectFeed(articleRepository.GetActiveSources()[0].ActiveFeeds[0]);
            }
        }

        public async void SelectFeed(FeedModel obj)
        {
            Feed = obj;
            await _articleRepository.LoadFullFeedAsync(obj);
        }
        
        public ArticleModel SelectedArticle
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    SimpleIoc.Default.GetInstance<ArticlePageViewModel>().SelectArticle(value);
                    _historyNavigationService.NavigateTo(PageKeys.Article.ToString());
                }
            }
        }
        
        private FeedModel _feed;
        public FeedModel Feed
        {
            get { return _feed; }
            set { Set(ref _feed, value); }
        }
    }
}
