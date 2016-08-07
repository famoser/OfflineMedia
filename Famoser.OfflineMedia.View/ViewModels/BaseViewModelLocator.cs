/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:OfflineMediaV3"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Repositories;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Repositories.Mocks;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.SqliteWrapper.Services;
using Famoser.SqliteWrapper.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Famoser.OfflineMedia.View.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class BaseViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static BaseViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IArticleRepository, ArticleRepositoryMock>();
                SimpleIoc.Default.Register<ISettingsRepository, SettingsRepositoryMock>();
                SimpleIoc.Default.Register<IThemeRepository, ThemeRepositoryMock>();
                SimpleIoc.Default.Register<IWeatherRepository, WeatherRepositoryMock>();
            }
            else
            {
                SimpleIoc.Default.Register<IArticleRepository, ArticleRepository>();
                SimpleIoc.Default.Register<ISettingsRepository, SettingsRepository>();
                SimpleIoc.Default.Register<IThemeRepository, ThemeRepository>();
                SimpleIoc.Default.Register<IWeatherRepository, WeatherRepository>();
            }

            SimpleIoc.Default.Register<ISqliteService, SqliteService>();
            SimpleIoc.Default.Register<IProgressService, ProgressService>();
            SimpleIoc.Default.Register<IImageDownloadService, ImageDownloadService>();

            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<FeedPageViewModel>();
            SimpleIoc.Default.Register<ArticlePageViewModel>();
            SimpleIoc.Default.Register<SettingsPageViewModel>();
            SimpleIoc.Default.Register<MyDayViewModel>();
        }

        public MainPageViewModel MainPageViewModel => ServiceLocator.Current.GetInstance<MainPageViewModel>();

        public FeedPageViewModel FeedPageViewModel => ServiceLocator.Current.GetInstance<FeedPageViewModel>();

        public ArticlePageViewModel ArticlePageViewModel => ServiceLocator.Current.GetInstance<ArticlePageViewModel>();

        public SettingsPageViewModel SettingsPageViewModel => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();

        public MyDayViewModel MyDayViewModel => ServiceLocator.Current.GetInstance<MyDayViewModel>();

        public ProgressService ProgressService => ServiceLocator.Current.GetInstance<IProgressService>() as ProgressService;

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}