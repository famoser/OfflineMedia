using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using GalaSoft.MvvmLight.Messaging;
using OfflineMedia.Business.Enums;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMedia.UserControls.ArticlePageControls
{
    public sealed partial class Read : UserControl
    {
        public Read()
        {
            this.InitializeComponent();
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Messenger.Default.Send(Messages.ScrollToTop);
        }
    }
}
