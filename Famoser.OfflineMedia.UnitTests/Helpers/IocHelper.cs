using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Repositories;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Repositories.Mocks;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.UnitTests.Services.Mocks;
using Famoser.SqliteWrapper.Services;
using Famoser.SqliteWrapper.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

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
            SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformWinRT>();
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
