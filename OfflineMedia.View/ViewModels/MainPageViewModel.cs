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
using OfflineMedia.Common.Framework.Timer;

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
            Messenger.Default.Register<List<ArticleModel>>(this, Messages.FeedRefresh, EvaluateMessages);
        }

        private async void EvaluateMessages(List<ArticleModel> obj)
        {
            var th = new TimerHelper();
            th.Stop("Evaluating List Message", this);
            var first = obj?.FirstOrDefault();
            if (Sources != null && first != null)
            {
                var source =
                    Sources.FirstOrDefault(s => s.SourceConfiguration == first.FeedConfiguration.SourceConfiguration);
                var feed = source?.FeedList.FirstOrDefault(f => f.FeedConfiguration == first.FeedConfiguration);
                if (feed != null)
                {
                    for (int i = 0; i < feed.ArticleList.Count && i < obj.Count && i < MaxArticlesPerFeed; i++)
                    {
                        feed.ArticleList[i] = obj[i];
                    }
                    for (int i = feed.ArticleList.Count; i < MaxArticlesPerFeed && i < obj.Count; i++)
                    {
                        feed.ArticleList.Add(obj[i]);
                    }

                    foreach (var articleModel in feed.ArticleList)
                    {
                        await _articleRepository.LoadMoreArticleContent(articleModel);
                    }
                }
            }
            th.Stop("Evaluating List Message finished", this);
            var res2 = th.GetAnalytics;
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
            TimerHelper.Instance.Stop("Initilizing...", this);
            _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.ReadingOutArticles);
            TimerHelper.Instance.Stop("Progress Showed, Sources start", this);
            Sources = await _articleRepository.GetSources();

            TimerHelper.Instance.Stop("Got Sources, Loading Feeds", this);
            foreach (var sourceModel in Sources)
            {
                foreach (var feedModel in sourceModel.FeedList)
                {
                    feedModel.ArticleList = await _articleRepository.GetArticlesByFeed(feedModel.FeedConfiguration.Guid, 5);
                    if (feedModel.ArticleList.Count > 0)
                    {
                        //load images
                        foreach (var articleModel in feedModel.ArticleList)
                        {
                            await _articleRepository.LoadMoreArticleContent(articleModel);
                        }
                    }
                    else
                        feedModel.ArticleList.Add(_articleRepository.GetEmptyFeedArticle());
                }
            }
            _favorites = await _settingsRepository.GetSettingByKey(SettingKeys.FavoritesEnabled);
            if (_favorites != null && _favorites.BoolValue)
            {
                TimerHelper.Instance.Stop("Got Feeds, Getting Favorites...", this);
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

                TimerHelper.Instance.Stop("Actualizing Articles", this);
                await _articleRepository.ActualizeArticles();
                TimerHelper.Instance.Stop("Uploading Stats", this);
                await _apiRepository.UploadStats();
                
                var res = TimerHelper.Instance.GetAnalytics;

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
