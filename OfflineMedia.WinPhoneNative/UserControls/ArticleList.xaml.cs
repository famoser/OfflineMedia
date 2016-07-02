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
        }
    }
}
