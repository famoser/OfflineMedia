using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.NewsModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMediaV3.UserControls
{
    public sealed partial class FullArticleList : UserControl
    {
        public FullArticleList()
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
