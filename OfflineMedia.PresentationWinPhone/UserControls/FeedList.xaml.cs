using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Models;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMedia.UserControls
{
    public sealed partial class FeedList : UserControl
    {
        public FeedList()
        {
            this.InitializeComponent();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        private readonly INavigationService _navigationService;

        private void FeedList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var feed = e.ClickedItem as FeedModel;
            _navigationService.NavigateTo(PageKeys.Feed.ToString());
            Messenger.Default.Send(feed, Messages.Select);
        }
    }
}
