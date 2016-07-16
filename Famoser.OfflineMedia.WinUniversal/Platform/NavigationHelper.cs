using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.OfflineMedia.View.Enums;
using Famoser.OfflineMedia.WinUniversal.Pages;

namespace Famoser.OfflineMedia.WinUniversal.Platform
{
    public static class NavigationHelper
    {
        public static HistoryNavigationService CreateNavigationService()
        {
            var navigationService = new HistoryNavigationService();

            navigationService.Configure(PageKeys.Main.ToString(), typeof(MainPage));
            navigationService.Configure(PageKeys.Feed.ToString(), typeof(FeedPage));
            navigationService.Configure(PageKeys.Article.ToString(), typeof(ArticlePage));
            navigationService.Configure(PageKeys.Settings.ToString(), typeof(SettingsPage));

            return navigationService;
        }
    }
}
