using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
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

            Messenger.Default.Register<ArticleModel>(this, Messages.ArticleRefresh, EvaluateMessages);
            Messenger.Default.Register<List<ArticleModel>>(this, Messages.FeedRefresh, EvaluateMessages);

            if (IsInDesignMode)
            {
                Feed = SimpleIoc.Default.GetInstance<IArticleRepository>().GetSampleArticles()[0].FeedList[0];
            }
        }

        private void EvaluateMessages(ArticleModel obj)
        {
            if (Feed != null && obj != null)
            {
                if (Feed.Source.SourceConfiguration.Source == SourceEnum.Favorites)
                {
                    if (obj.IsFavorite)
                    {
                        if (Feed.ArticleList.Any(a => a.Id != obj.Id))
                            Feed.ArticleList.Insert(0, obj);
                    }
                    else
                    {
                        var am = Feed.ArticleList.FirstOrDefault(d => d.Id == obj.Id);
                        if (am != null)
                            Feed.ArticleList.Remove(am);
                    }
                }
                var oldarticle = Feed.ArticleList.FirstOrDefault(a => a.Id == obj.Id);
                if (oldarticle != null)
                {
                    var index = Feed.ArticleList.IndexOf(oldarticle);
                    Feed.ArticleList[index] = obj;
                }
            }
        }

        private void EvaluateMessages(List<ArticleModel> obj)
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

                    _articleRepository.AddListProperties(Feed.ArticleList.ToList());
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
                Feed.ArticleList = await _articleRepository.GetArticlesByFeed(obj.FeedConfiguration.Guid, 1, 0);
                if (Feed.ArticleList.Count == 1)
                {
                    //load rest of articles
                    for (int i = 1; ; i++)
                    {
                        var newarticle =
                            (await _articleRepository.GetArticlesByFeed(obj.FeedConfiguration.Guid, 1, i))
                                .FirstOrDefault();
                        if (newarticle != null)
                            Feed.ArticleList.Add(newarticle);
                        else
                            break;
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
