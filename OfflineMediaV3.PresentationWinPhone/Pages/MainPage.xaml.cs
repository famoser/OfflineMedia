using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using OfflineMediaV3.InfoPage;
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
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (!e.Handled)
            { 
                if (_open)
                {
                    Button_Tapped(null, null);
                    e.Handled = true;
                }
                else
                {
                    Application.Current.Exit();
                }
            }
        }

        private bool _open = false;
        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (_open)
                Close.Begin();
            else
                Open.Begin();
            _open = !_open;
        }
    }
}
