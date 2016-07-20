using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.DebugTools;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using Famoser.OfflineMedia.View.Helpers;
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
        private readonly IProgressService _progressService;
        private readonly IArticleRepository _articleRepository;
        private ISettingsRepository _settingsRepository;
        private IDialogService _dialogService;
        private const int MaxArticlesPerFeed = 5;

        private readonly IHistoryNavigationService _historyNavigationService;

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
            _selectArticleCommand = new RelayCommand<ArticleModel>(SelectArticle);

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

        private RelayCommand<ArticleModel> _selectArticleCommand;
        public ICommand SelectArticleCommand => _selectArticleCommand;

        private void SelectArticle(ArticleModel model)
        {
            SimpleIoc.Default.GetInstance<ArticlePageViewModel>().SelectArticle(model);
            _historyNavigationService.NavigateTo(PageKeys.Article.ToString());
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

        private readonly RelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;
        private bool CanRefresh => !_isRefreshing;
        private bool _isRefreshing;
        private async void Refresh()
        {
            using (new LoadingCommand(_refreshCommand, (b) => _isRefreshing = b, IndeterminateProgressKey.RefreshingArticles, _progressService))
            {
                TimerHelper.Instance.Stop("Actualizing Articles", this);
                await _articleRepository.ActualizeAllArticlesAsync();
                var res = TimerHelper.Instance.GetAnalytics;
            }
        }

        #endregion

    }
}
