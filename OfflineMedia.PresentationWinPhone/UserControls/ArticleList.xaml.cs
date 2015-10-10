using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;

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
    }
}
