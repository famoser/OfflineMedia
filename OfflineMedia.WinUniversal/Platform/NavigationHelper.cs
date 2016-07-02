using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight.Views;
using OfflineMedia.WinUniversal;

namespace OfflineMedia.Platform
{
    public static class NavigationHelper
    {
        public static INavigationService CreateNavigationService()
        {
            var navigationService = new CustomNavigationService();

            navigationService.Implementation.Configure(PageKeys.Main.ToString(), typeof(MainPage));
            //navigationService.Configure(PageKeys.Feed.ToString(), typeof(FeedPage));
            //navigationService.Configure(PageKeys.Article.ToString(), typeof(NewArticlePage));
            //navigationService.Configure(PageKeys.Settings.ToString(), typeof(SettingsPage));

            return navigationService;
        }
    }
}
