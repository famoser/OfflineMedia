using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.NewsModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMediaV3.UserControls
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
            _navigationService.NavigateTo(PageKeys.Article.ToString());
            Messenger.Default.Send(article, Messages.Select);
        }
    }
}
