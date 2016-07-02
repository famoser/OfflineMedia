using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class FeedPageViewModel : ViewModelBase
    {
        private IArticleRepository _articleRepository;
        public FeedPageViewModel(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;

            Messenger.Default.Register<FeedModel>(this, Messages.Select, EvaluateSelect);

            if (IsInDesignMode)
            {
                Feed = articleRepository.GetSampleSources()[0].ActiveFeeds[0];
            }
        }
        
        private async void EvaluateSelect(FeedModel obj)
        {
            Feed = obj;
            await _articleRepository.LoadFullFeedAsync(obj);
        }

        private FeedModel _feed;
        public FeedModel Feed
        {
            get { return _feed; }
            set { Set(ref _feed, value); }
        }
    }
}
