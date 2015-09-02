using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using OfflineMediaV3.View.ViewModels;
using OfflineMediaV3.View.ViewModels.Global;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace OfflineMediaV3.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private ProgressViewModel _viewModel = SimpleIoc.Default.GetInstance<ProgressViewModel>();
        private MainPageViewModel _mainPageViewModel = SimpleIoc.Default.GetInstance<MainPageViewModel>();

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //State["ScrolPos"] = 12;
            base.OnNavigatedTo(e);
            if (_mainPageViewModel.Sources != null)
                foreach (var sources in _mainPageViewModel.Sources)
                {
                    if (sources.FeedList != null)
                        foreach (var feedModel in sources.FeedList)
                        {
                            if (feedModel.ArticleList != null)
                            {
                                feedModel.RefreshArticleList();
                            }
                        }
                }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
    }
}
