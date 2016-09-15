using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.DebugTools;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.FrameworkEssentials.View.Interfaces;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using IndeterminateProgressKey = Famoser.OfflineMedia.View.Enums.IndeterminateProgressKey;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class MainPageViewModel : ViewModelBase, INavigationBackNotifier
    {
        private readonly IProgressService _progressService;
        private readonly IArticleRepository _articleRepository;
        private ISettingsRepository _settingsRepository;
        private IDialogService _dialogService;
        private IPermissionsService _permissionsService;
        private const int MaxArticlesPerFeed = 5;

        private readonly IHistoryNavigationService _historyNavigationService;

        public MainPageViewModel(IProgressService progressService, IArticleRepository articleRepository, ISettingsRepository settingsRepository, IHistoryNavigationService historyNavigationService, IDialogService dialogService, IPermissionsService permissionsService)
        {
            _progressService = progressService;
            _articleRepository = articleRepository;
            _settingsRepository = settingsRepository;
            _historyNavigationService = historyNavigationService;
            _dialogService = dialogService;
            _permissionsService = permissionsService;

            _openSettingsCommand = new RelayCommand(OpenSettings);
            _openInfoCommand = new RelayCommand(OpenInfo);
            _refreshCommand = new LoadingRelayCommand(Refresh, () => _canRefresh, true);
            _selectArticleCommand = new RelayCommand<ArticleModel>(SelectArticle);
            _selectFeedCommand = new RelayCommand<FeedModel>(SelectFeed);

            _permissionsService.PermissionsChanged += PermissionsServiceOnPermissionsChanged;

            Sources = _articleRepository.GetActiveSources();

            if (!IsInDesignMode)
            {
                Initialize();
            }
        }

        private async void PermissionsServiceOnPermissionsChanged(object sender, EventArgs eventArgs)
        {
            _canRefresh = await _permissionsService.CanDownload();
            _refreshCommand.RaiseCanExecuteChanged();
        }

        private async void Initialize()
        {
            _canRefresh = await _permissionsService.CanDownload();
            _refreshCommand.RaiseCanExecuteChanged();
        }

        public ObservableCollection<SourceModel> Sources { get; }

        private readonly RelayCommand<ArticleModel> _selectArticleCommand;
        public ICommand SelectArticleCommand => _selectArticleCommand;

        private void SelectArticle(ArticleModel model)
        {
            SimpleIoc.Default.GetInstance<ArticlePageViewModel>().SelectArticle(model);
            _historyNavigationService.NavigateTo(PageKeys.Article.ToString());
        }

        private readonly RelayCommand<FeedModel> _selectFeedCommand;
        public ICommand SelectFeedCommand => _selectFeedCommand;

        private void SelectFeed(FeedModel model)
        {
            SimpleIoc.Default.GetInstance<FeedPageViewModel>().SelectFeed(model);
            _historyNavigationService.NavigateTo(PageKeys.Feed.ToString());
        }


        #region open settings
        private readonly RelayCommand _openSettingsCommand;
        public ICommand OpenSettingsCommand => _openSettingsCommand;
        private void OpenSettings()
        {
            _historyNavigationService.NavigateTo(PageKeys.Settings.ToString());
        }
        #endregion

        #region open info
        private readonly RelayCommand _openInfoCommand;
        public ICommand OpenInfoCommand => _openInfoCommand;
        private void OpenInfo()
        {
            var avm = SimpleIoc.Default.GetInstance<ArticlePageViewModel>();
            avm.SelectArticle(_articleRepository.GetInfoArticle());
            _historyNavigationService.NavigateTo(PageKeys.Article.ToString());
        }
        #endregion

        #region refresh

        private readonly LoadingRelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;
        private bool _canRefresh;
        private async Task Refresh()
        {
            _canRefresh = await _permissionsService.CanDownload();
            _refreshCommand.RaiseCanExecuteChanged();
            if (_canRefresh)
                await _articleRepository.ActualizeAllArticlesAsync();
        }

        #endregion

        public void HandleNavigationBack(object message)
        {
            _refreshCommand.RaiseCanExecuteChanged();
        }
    }
}
