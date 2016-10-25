using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.OfflineMedia.View.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Famoser.OfflineMedia.WinUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void AppBar_OnOpening(object sender, object e)
        {
            var vm = DataContext as MainPageViewModel;
            var com = vm?.RefreshCommand as LoadingRelayCommand;
            com?.RaiseCanExecuteChanged();
        }
    }
}
