using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using OfflineMedia.Business.Enums;

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
            Messenger.Default.Register<Messages>(this, ScrollToTop);
        }

        private void ScrollToTop(Messages msg)
        {
            if (msg == Messages.ScrollToTop)
                ReadScrollViewer.ChangeView(null, 0, null);
        }
    }
}
