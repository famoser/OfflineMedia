using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace OfflineMedia.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewArticlePage : Page
    {
        public NewArticlePage()
        {
            this.InitializeComponent();
        }

        private bool _upButtonVisible = false;
        private void ScrollViewer_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            if (sv != null)
            {
                if (sv.VerticalOffset > 1000)
                {
                    if (!_upButtonVisible)
                    {
                        ShowUpButton.Begin();
                        _upButtonVisible = !_upButtonVisible;
                    }
                }
                else
                {
                    if (_upButtonVisible)
                    {
                        HideUpButton.Begin();
                        _upButtonVisible = !_upButtonVisible;
                    }
                }
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            ReadScrollViewer.ChangeView(null, 0, null);
            HideUpButton.Begin();
            _upButtonVisible = !_upButtonVisible;
        }
    }
}
