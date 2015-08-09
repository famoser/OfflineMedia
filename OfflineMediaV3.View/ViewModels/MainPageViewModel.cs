using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
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
                Sources = _articleRepository.GetSampleArticles();
            else
                Initialize();

            Messenger.Default.Register<Guid>(this,Messages.FeedRefresh, FeedRefreshed);
        }

        private async void FeedRefreshed(Guid obj)
        {
            foreach (var sourceModel in Sources)
            {
                foreach (var feed in sourceModel.FeedList)
                {
                    if (feed.FeedConfiguration.Guid == obj)
                        feed.ShortArticleList = await _articleRepository.GetArticlesByFeed(obj, 5);
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
            ActualizeArticles();
        }

        #endregion

        public async void Initialize()
        {
            _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.ReadingOutArticles);
            Sources = await _articleRepository.GetSources();
            _progressService.HideIndeterminateProgress(IndeterminateProgressKey.ReadingOutArticles);
            ActualizeArticles();
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
