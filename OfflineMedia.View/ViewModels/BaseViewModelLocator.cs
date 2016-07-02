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

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using OfflineMedia.Business.Repositories;
using OfflineMedia.Business.Repositories.Interfaces;

namespace OfflineMedia.View.ViewModels
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
        public BaseViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IArticleRepository, ArticleRepository>();
            SimpleIoc.Default.Register<ISettingsRepository, SettingsRepository>();
            SimpleIoc.Default.Register<IThemeRepository, ThemeRepository>();
            SimpleIoc.Default.Register<IWeatherRepository, WeatherRepository>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
            }

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

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}