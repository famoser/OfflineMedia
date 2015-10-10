﻿using GalaSoft.MvvmLight.Views;
using OfflineMedia.Business.Enums;

namespace OfflineMedia.Platform
{
    public static class NavigationHelper
    {
        public static INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();

            navigationService.Configure(PageKeys.Main.ToString(), typeof(MainPage));
            //navigationService.Configure(PageKeys.Feed.ToString(), typeof(FeedPage));
            //navigationService.Configure(PageKeys.Article.ToString(), typeof(NewArticlePage));
            //navigationService.Configure(PageKeys.Settings.ToString(), typeof(SettingsPage));

            return navigationService;
        }
    }
}