using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Repositories.Mocks;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.UnitTests.Services.Mocks;
using Famoser.SqliteWrapper.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SQLite.Net.Interop;

namespace Famoser.OfflineMedia.UnitTests.Helpers
{
    public class IocHelper
    {
        public static void InitializeContainer()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // services
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IPlatformCodeService, PlatformCodeServiceMock>();
            SimpleIoc.Default.Register<IStorageService>(() => new StorageServiceMock());
            SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformMock>();
            SimpleIoc.Default.Register<ISqliteServiceSettingsProvider, SqliteServiceSettingsProviderMock>();
            SimpleIoc.Default.Register<IHistoryNavigationService, HistoryNavigationServiceMock>();

            //repos
            SimpleIoc.Default.Register<IThemeRepository, ThemeRepositoryMock>();
            SimpleIoc.Default.Register<IArticleRepository, ArticleRepositoryMock>();
            SimpleIoc.Default.Register<IWeatherRepository, WeatherRepositoryMock>();
            SimpleIoc.Default.Register<ISettingsRepository, SettingsRepositoryMock>();
        }
    }
}
