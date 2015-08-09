using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Framework.Repositories;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.View.ViewModels
{
    public class FeedPageViewModel : ViewModelBase
    {
        private IArticleRepository _articleRepository;
        public FeedPageViewModel(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;

            Messenger.Default.Register<FeedModel>(this, Messages.Select, EvaluateSelect);
            Messenger.Default.Register<Guid>(this, Messages.FeedRefresh, Refreshed);

            if (IsInDesignMode)
            {
                Feed = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];
            }
        }

        private async void Refreshed(Guid obj)
        {
            if (obj == Feed.FeedConfiguration.Guid)
            {
                Feed.ArticleList = await _articleRepository.GetArticleByFeed(obj);
            }
        }

        private async void EvaluateSelect(FeedModel obj)
        {
            Feed = obj;
            Feed.ArticleList = await _articleRepository.GetArticleByFeed(obj.FeedConfiguration.Guid);
        }

        private FeedModel _feed;
        public FeedModel Feed
        {
            get { return _feed; }
            set { Set(ref _feed, value); }
        }
    }
}
