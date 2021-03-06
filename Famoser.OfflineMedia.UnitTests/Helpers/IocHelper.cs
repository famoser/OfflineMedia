﻿using CommonServiceLocator;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Repositories;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Repositories.Mocks;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.UnitTests.Local;
using Famoser.OfflineMedia.UnitTests.Services.Mocks;
using Famoser.SqliteWrapper.Services;
using Famoser.SqliteWrapper.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;


namespace Famoser.OfflineMedia.UnitTests.Helpers
{
    public class IocHelper
    {
        private static bool _isIntialized;
        public static void InitializeContainer()
        {
            if (_isIntialized)
                return;
            _isIntialized = true;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // services
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IPlatformCodeService, PlatformCodeService>();
            SimpleIoc.Default.Register<IPermissionsService, PermissionService>();
            SimpleIoc.Default.Register<OfflineMedia.Business.Services.Interfaces.IProgressService, ProgressionService>();
            SimpleIoc.Default.Register<IStorageService>(() => new StorageServiceMock());
            SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformWinRT>();
            SimpleIoc.Default.Register<IImageDownloadService, ImageDownloadService>();
            SimpleIoc.Default.Register<ISqliteServiceSettingsProvider, SqliteServiceSettingsProviderMock>();
            SimpleIoc.Default.Register<IHistoryNavigationService, HistoryNavigationServiceMock>();
            SimpleIoc.Default.Register<ISqliteService>(() => new SqliteService(SimpleIoc.Default.GetInstance<ISQLitePlatform>(), SimpleIoc.Default.GetInstance<ISqliteServiceSettingsProvider>()));


            //repos
            SimpleIoc.Default.Register<IThemeRepository, ThemeRepository>();
            SimpleIoc.Default.Register<IArticleRepository, ArticleRepository>();
            SimpleIoc.Default.Register<IWeatherRepository, WeatherRepository>();
            SimpleIoc.Default.Register<ISettingsRepository, SettingsRepositoryMock>();
        }
    }
}
