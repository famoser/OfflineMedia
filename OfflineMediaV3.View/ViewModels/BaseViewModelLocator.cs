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
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Repositories;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Data;
using OfflineMediaV3.View.ViewModels.Global;

namespace OfflineMediaV3.View.ViewModels
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
            SimpleIoc.Default.Register<IApiRepository, ApiRepository>();

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
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }

        public FeedPageViewModel FeedPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FeedPageViewModel>();
            }
        }

        public ArticlePageViewModel ArticlePageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ArticlePageViewModel>();
            }
        }

        public SettingsPageViewModel SettingsPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsPageViewModel>();
            }
        }

        public ProgressViewModel ProgressViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProgressViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}