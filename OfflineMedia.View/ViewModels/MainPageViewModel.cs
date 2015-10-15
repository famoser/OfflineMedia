using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Common.Enums.View;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;

namespace OfflineMedia.View.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IProgressService _progressService;
        private IArticleRepository _articleRepository;
        private ISettingsRepository _settingsRepository;
        private IApiRepository _apiRepository;
        private IDialogService _dialogService;
        private const int MaxArticlesPerFeed = 5;

        private INavigationService _navigationService;

        public MainPageViewModel(IProgressService progressService, IArticleRepository articleRepository, ISettingsRepository settingsRepository, INavigationService navigationService, IApiRepository apiRepository, IDialogService dialogService)
        {
            _progressService = progressService;
            _articleRepository = articleRepository;
            _settingsRepository = settingsRepository;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _apiRepository = apiRepository;

            _openSettingsCommand = new RelayCommand(OpenSettings);
            _openInfoCommand = new RelayCommand(OpenInfo);
            _refreshCommand = new RelayCommand(Refresh, () => CanRefresh);

            if (IsInDesignMode)
            {
                Sources = _articleRepository.GetSampleArticles();
                Sources[0].FeedList[0].ArticleList[0].State = ArticleState.New;
                Sources[0].FeedList[0].ArticleList[1].State = ArticleState.Loading;
                Sources[0].FeedList[0].ArticleList[2].State = ArticleState.Loaded;
                Sources[0].FeedList[0].ArticleList[3].State = ArticleState.Read;
            }
            else
                Initialize();

            Messenger.Default.Register<PageKeys>(this, Messages.ReloadGoBackPage, ReloadPage);
            Messenger.Default.Register<ArticleModel>(this, Messages.ArticleRefresh, EvaluateMessages);
            Messenger.Default.Register<List<ArticleModel>>(this, Messages.FeedRefresh, EvaluateMessages);
        }

        private void EvaluateMessages(ArticleModel obj)
        {
            if (Sources != null && obj != null)
            {
                if (_favorites != null && _favorites.BoolValue && Sources.Any() && Sources[Sources.Count - 1].SourceConfiguration.Source == SourceEnum.Favorites)
                {
                    if (obj.IsFavorite)
                    {
                        if (Sources[Sources.Count - 1].FeedList[0].ArticleList.Any(a => a.Id != obj.Id))
                            Sources[Sources.Count - 1].FeedList[0].ArticleList.Insert(0, obj);
                    }
                    else
                    {
                        var am = Sources[Sources.Count - 1].FeedList[0].ArticleList.FirstOrDefault(d => d.Id == obj.Id);
                        if (am != null)
                            Sources[Sources.Count - 1].FeedList[0].ArticleList.Remove(am);
                    }
                }
                foreach (var sourceModel in Sources)
                {
                    if (sourceModel.FeedList != null)
                    {
                        foreach (var feedModel in sourceModel.FeedList)
                        {
                            var oldarticle = feedModel.ArticleList.FirstOrDefault(a => a.Id == obj.Id);
                            if (oldarticle != null)
                            {
                                var index = feedModel.ArticleList.IndexOf(oldarticle);
                                feedModel.ArticleList[index] = obj;
                            }
                        }
                    }
                }
            }
        }

        private void EvaluateMessages(List<ArticleModel> obj)
        {
            var first = obj?.FirstOrDefault();
            if (Sources != null && first != null)
            {
                foreach (var sourceModel in Sources)
                {
                    if (sourceModel.FeedList != null)
                    {
                        foreach (var feedModel in sourceModel.FeedList)
                        {
                            if (feedModel.FeedConfiguration.Guid == first.FeedConfigurationId)
                            {
                                if (obj.Count < MaxArticlesPerFeed)
                                {
                                    var oldcount = obj.Count;
                                    for (int i = obj.Count; i < MaxArticlesPerFeed; i++)
                                    {
                                        if (feedModel.ArticleList.Count > i - oldcount)
                                            obj.Add(feedModel.ArticleList[i - oldcount]);
                                        else
                                            break;
                                    }
                                }

                                for (int i = 0; i < feedModel.ArticleList.Count; i++)
                                {
                                    if (obj.Count > i)
                                        feedModel.ArticleList[i] = obj[i];
                                    else
                                        feedModel.ArticleList.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReloadPage(PageKeys obj)
        {
            if (obj == PageKeys.Main)
                Initialize();
        }

        private ObservableCollection<SourceModel> _sources;
        public ObservableCollection<SourceModel> Sources
        {
            get { return _sources; }
            set { Set(ref _sources, value); }
        }


        #region open settings

        private RelayCommand _openSettingsCommand;
        public ICommand OpenSettingsCommand
        {
            get { return _openSettingsCommand; }
        }

        private void OpenSettings()
        {
            _navigationService.NavigateTo(PageKeys.Settings.ToString());
        }

        #endregion

        #region open info

        private RelayCommand _openInfoCommand;
        public ICommand OpenInfoCommand
        {
            get { return _openInfoCommand; }
        }

        private void OpenInfo()
        {
            _navigationService.NavigateTo(PageKeys.Article.ToString());
            Messenger.Default.Send(_articleRepository.GetInfoArticle(), Messages.Select);
        }

        #endregion

        #region refresh

        private RelayCommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get { return _refreshCommand; }
        }

        private bool CanRefresh
        {
            get { return !_isActualizing; }
        }

        private void Refresh()
        {
            Messenger.Default.Send(Messages.RefreshWeather);
            ActualizeArticles();
        }

        #endregion

        private SettingModel _favorites;
        public async void Initialize()
        {
            _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.ReadingOutArticles);
            Sources = await _articleRepository.GetSources();
            foreach (var sourceModel in Sources)
            {
                foreach (var feedModel in sourceModel.FeedList)
                {
                    feedModel.ArticleList = await _articleRepository.GetArticlesByFeed(feedModel.FeedConfiguration.Guid, 1);
                    if (feedModel.ArticleList.Count == 1)
                    {
                        //load rest of articles
                        for (int i = 1; i < MaxArticlesPerFeed; i++)
                        {
                            var newarticle =
                                (await _articleRepository.GetArticlesByFeed(feedModel.FeedConfiguration.Guid, 1, i))
                                    .FirstOrDefault();
                            if (newarticle != null)
                                feedModel.ArticleList.Add(newarticle);
                            else
                                break;
                        }
                    }
                    else
                        feedModel.ArticleList.Add(_articleRepository.GetEmptyFeedArticle());
                }
            }
            _favorites = await _settingsRepository.GetSettingByKey(SettingKeys.FavoritesEnabled);
            if (_favorites != null && _favorites.BoolValue)
            {
                Sources.Add(await _articleRepository.GetFavorites());
            }

            Messenger.Default.Send(Messages.MainPageInitialized);

            ActualizeArticles();
            _progressService.HideIndeterminateProgress(IndeterminateProgressKey.ReadingOutArticles);
        }

        private bool _isActualizing;
        private async void ActualizeArticles()
        {
            try
            {
                if (_isActualizing)
                    return;

                _isActualizing = true;
                _refreshCommand.RaiseCanExecuteChanged();

                await _articleRepository.ActualizeArticles();
                await _apiRepository.UploadStats();

                _progressService.HideProgress();
                _progressService.ShowDecentInformationMessage("Aktualisierung abgeschlossen", TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "ActualizeArticle failed", ex);
                _progressService.HideProgress();
                _progressService.ShowDecentInformationMessage("Aktualisierung fehlgeschlagen", TimeSpan.FromSeconds(3));
            }

            _isActualizing = false;
            _refreshCommand.RaiseCanExecuteChanged();
        }
    }
}
