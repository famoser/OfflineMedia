using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Famoser.OfflineMedia.WinUniversal.Platform;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Famoser.OfflineMedia.WinUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            ArticleDownloadCheckBox.IsChecked = await PermissionService.GetArticleDownloadOnMobileConnection();
            ImageDownloadCheckBox.IsChecked = await PermissionService.GetImageDownloadOnMobileConnection();
        }

        private async void ArticleDownloadClicked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb != null)
                await PermissionService.SetArticleDownloadOnMobileConnection(cb.IsChecked.HasValue && cb.IsChecked.Value);
        }

        private async void ImageDownloadClicked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb != null)
                await PermissionService.SetImageDownloadOnMobileConnection(cb.IsChecked.HasValue && cb.IsChecked.Value);
        }
    }
}
