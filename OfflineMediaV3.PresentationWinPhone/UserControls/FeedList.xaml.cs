using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.NewsModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMediaV3.UserControls
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
