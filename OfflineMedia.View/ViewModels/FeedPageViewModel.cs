using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Nito.AsyncEx;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.View.ViewModels
{
    public class FeedPageViewModel : ViewModelBase
    {
        private IArticleRepository _articleRepository;
        public FeedPageViewModel(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;

            Messenger.Default.Register<FeedModel>(this, Messages.Select, EvaluateSelect);
            Messenger.Default.Register<List<ArticleModel>>(this, Messages.FeedRefresh, EvaluateMessages);

            if (IsInDesignMode)
            {
                Feed = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];
            }
        }

        private async void EvaluateMessages(List<ArticleModel> obj)
        {
            var first = obj?.FirstOrDefault();
            if (Feed != null && first != null)
            {
                if (Feed.FeedConfiguration.Guid == first.FeedConfigurationId)
                {
                    for (int i = 0; i < Feed.ArticleList.Count; i++)
                    {
                        if (obj.Count > i)
                            Feed.ArticleList[i] = obj[i];
                        else
                            Feed.ArticleList.RemoveAt(i);
                    }
                    if (obj.Count > Feed.ArticleList.Count)
                    {
                        for (int i = Feed.ArticleList.Count; i < obj.Count; i++)
                        {
                            Feed.ArticleList.Add(obj[i]);
                        }
                    }
                }
            }
        }

        private async void EvaluateSelect(FeedModel obj)
        {
            if (_lastConfig != obj.FeedConfiguration)
            {
                _lastConfig = obj.FeedConfiguration;
                Feed = new FeedModel
                {
                    FeedConfiguration = obj.FeedConfiguration,
                    Source = obj.Source,
                    ArticleList = new ObservableCollection<ArticleModel>()
                };
                Feed.ArticleList = await _articleRepository.GetArticlesByFeed(obj.FeedConfiguration.Guid, 40);
                if (Feed.ArticleList.Count > 0)
                {
                    foreach (var articleModel in Feed.ArticleList)
                    {
                        await _articleRepository.LoadMoreArticleContent(articleModel);
                    }
                }
                else
                    Feed.ArticleList.Add(_articleRepository.GetEmptyFeedArticle());
            }
        }

        private FeedConfigurationModel _lastConfig;

        private FeedModel _feed;
        public FeedModel Feed
        {
            get { return _feed; }
            set { Set(ref _feed, value); }
        }
    }
}
