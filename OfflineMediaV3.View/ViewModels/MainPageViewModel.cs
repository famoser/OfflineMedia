using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Enums.Settings;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Enums.View;
using OfflineMediaV3.Common.Framework.Logs;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;

namespace OfflineMediaV3.View.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IProgressService _progressService;
        private IArticleRepository _articleRepository;
        private ISettingsRepository _settingsRepository;
        private IApiRepository _apiRepository;
        private IDialogService _dialogService;

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

            Messenger.Default.Register<Guid>(this, Messages.FeedRefresh, FeedRefreshed);
            Messenger.Default.Register<int>(this, Messages.FeedArticleRefresh, ArticleRefreshed);
            Messenger.Default.Register<PageKeys>(this, Messages.ReloadGoBackPage, ReloadPage);
            Messenger.Default.Register<Messages>(this, EvaluateMessages);
        }

        private async void ArticleRefreshed(int obj)
        {
            if (Sources != null)
            {
                foreach (var sourceModel in Sources)
                {
                    if (sourceModel.FeedList != null)
                    {
                        foreach (var feedModel in sourceModel.FeedList)
                        {

                            for (int index = 0; index < feedModel.ArticleList.Count; index++)
                            {
                                if (feedModel.ArticleList[index].Id == obj)
                                    feedModel.ArticleList[index] = await _articleRepository.GetArticleById(obj);
                            }
                        }
                    }
                }
            }
        }

        private async void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.FavoritesChanged)
            {
                if (Sources.Any() && _favorites.BoolValue)
                {
                    Sources[Sources.Count - 1] = await _articleRepository.GetFavorites();
                }
            }
        }

        private void ReloadPage(PageKeys obj)
        {
            if (obj == PageKeys.Main)
                Initialize();
        }

        private async void FeedRefreshed(Guid obj)
        {
            foreach (var sourceModel in Sources)
            {
                foreach (var feed in sourceModel.FeedList)
                {
                    if (feed.FeedConfiguration.Guid == obj)
                        feed.ArticleList = await _articleRepository.GetArticlesByFeed(obj, 5);
                }
            }
        }

        private ObservableCollection<SourceModel> _sources;
        public ObservableCollection<SourceModel> Sources
        {
            get { return _sources; }
            set { Set(ref _sources, value); }
        }


        #region open settings

        private RelayCommand _openSettingsCommand;
        public ICommand OpenSettingsCommand => _openSettingsCommand;

        private void OpenSettings()
        {
            _navigationService.NavigateTo(PageKeys.Settings.ToString());
        }

        #endregion

        #region open info

        private RelayCommand _openInfoCommand;
        public ICommand OpenInfoCommand => _openInfoCommand;

        private void OpenInfo()
        {
            _navigationService.NavigateTo(PageKeys.Article.ToString());
            Messenger.Default.Send(_articleRepository.GetInfoArticle(), Messages.Select);
        }

        #endregion

        #region refresh

        private RelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;

        private bool CanRefresh => !_isActualizing;

        private void Refresh()
        {
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
                    feedModel.ArticleList = await _articleRepository.GetArticlesByFeed(feedModel.FeedConfiguration.Guid, 5);
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

                await _articleRepository.ActualizeArticles(_progressService);
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
