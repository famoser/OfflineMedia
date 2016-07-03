using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.DebugTools;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using IndeterminateProgressKey = Famoser.OfflineMedia.View.Enums.IndeterminateProgressKey;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IProgressService _progressService;
        private IArticleRepository _articleRepository;
        private ISettingsRepository _settingsRepository;
        private IDialogService _dialogService;
        private const int MaxArticlesPerFeed = 5;

        private IHistoryNavigationService _historyNavigationService;

        public MainPageViewModel(IProgressService progressService, IArticleRepository articleRepository, ISettingsRepository settingsRepository, IHistoryNavigationService historyNavigationService, IDialogService dialogService)
        {
            _progressService = progressService;
            _articleRepository = articleRepository;
            _settingsRepository = settingsRepository;
            _historyNavigationService = historyNavigationService;
            _dialogService = dialogService;

            _openSettingsCommand = new RelayCommand(OpenSettings);
            _openInfoCommand = new RelayCommand(OpenInfo);
            _refreshCommand = new RelayCommand(Refresh, () => CanRefresh);
            
            Sources = _articleRepository.GetActiveSources();
            if (!IsInDesignMode)
                Refresh();
        }

        private ObservableCollection<SourceModel> _sources;
        public ObservableCollection<SourceModel> Sources
        {
            get { return _sources; }
            set { Set(ref _sources, value); }
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

        public FeedModel SelectedFeed
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    SimpleIoc.Default.GetInstance<FeedPageViewModel>().SelectFeed(value);
                    _historyNavigationService.NavigateTo(PageKeys.Feed.ToString());
                }
            }
        }


        #region open settings

        private RelayCommand _openSettingsCommand;
        public ICommand OpenSettingsCommand => _openSettingsCommand;

        private void OpenSettings()
        {
            _historyNavigationService.NavigateTo(PageKeys.Settings.ToString());
        }

        #endregion

        #region open info

        private RelayCommand _openInfoCommand;
        public ICommand OpenInfoCommand => _openInfoCommand;

        private void OpenInfo()
        {
            _historyNavigationService.NavigateTo(PageKeys.Article.ToString());
            Messenger.Default.Send(_articleRepository.GetInfoArticle(), Messages.Select);
        }

        #endregion

        #region refresh

        private RelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;

        private bool CanRefresh => !_isActualizing;

        private bool _isActualizing;
        private async void Refresh()
        {
            if (_isActualizing)
                return;

            _isActualizing = true;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.StartIndeterminateProgress(IndeterminateProgressKey.RefreshingArticles);

            TimerHelper.Instance.Stop("Actualizing Articles", this);
            await _articleRepository.ActualizeAllArticlesAsync();
            TimerHelper.Instance.Stop("Uploading Stats", this);
            _progressService.StopIndeterminateProgress(IndeterminateProgressKey.RefreshingArticles);

            var res = TimerHelper.Instance.GetAnalytics;

            _isActualizing = false;
            _refreshCommand.RaiseCanExecuteChanged();
        }

        #endregion

    }
}
