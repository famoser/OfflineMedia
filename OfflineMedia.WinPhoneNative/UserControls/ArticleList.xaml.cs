using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMedia.UserControls
{
    public sealed partial class ArticleList : UserControl
    {
        public ArticleList()
        {
            this.InitializeComponent();
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        private readonly INavigationService _navigationService;

        private void ArticleList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var article = e.ClickedItem as ArticleModel;

            var fm = DataContext as FeedModel;
            if (fm?.FeedConfiguration != null)
            {
                _navigationService.NavigateTo(PageKeys.Article.ToString());
                Messenger.Default.Send(article, Messages.Select);
            }
            else
            {
                Messenger.Default.Send(article, Messages.Select);
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var feed = DataContext as FeedModel;

            if (feed?.FeedConfiguration != null)
            {
                _navigationService.NavigateTo(PageKeys.Feed.ToString());
                Messenger.Default.Send(feed, Messages.Select);
            }
        }
    }
}
