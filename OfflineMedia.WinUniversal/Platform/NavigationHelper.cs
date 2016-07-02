using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.OfflineMedia.View.Enums;
using OfflineMedia.WinUniversal.Pages;

namespace OfflineMedia.WinUniversal.Platform
{
    public static class NavigationHelper
    {
        public static IHistoryNavigationService CreateNavigationService()
        {
            var navigationService = new HistoryNavigationServices();

            navigationService.Configure(PageKeys.Main.ToString(), typeof(MainPage));
            navigationService.Configure(PageKeys.Feed.ToString(), typeof(FeedPage));
            navigationService.Configure(PageKeys.Article.ToString(), typeof(NewArticlePage));
            navigationService.Configure(PageKeys.Settings.ToString(), typeof(SettingsPage));

            return navigationService;
        }
    }
}
