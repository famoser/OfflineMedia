using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.View.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace OfflineMediaV3.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArticlePage : Page
    {
        public ArticlePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void red_LayoutUpdated(object sender, object e)
        {
            var tb = sender as TextBlock;
            if (tb != null)
            {
                var val = tb.ActualWidth;

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewmodel = this.DataContext as ArticlePageViewModel;
            if (viewmodel != null)
            {
                Launcher.LaunchUriAsync(viewmodel.Article.PublicUri);
            }
        }
    }
}
