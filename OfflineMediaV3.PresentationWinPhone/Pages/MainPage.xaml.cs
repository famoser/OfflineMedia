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
    }
}
