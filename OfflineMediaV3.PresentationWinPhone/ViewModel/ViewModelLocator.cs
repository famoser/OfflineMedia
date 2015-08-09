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
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Pages;
using OfflineMediaV3.Services;
using OfflineMediaV3.View.ViewModels;
using OfflineMediaV3.View.ViewModels.Global;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace OfflineMediaV3.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator : BaseViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Create design time view services and models
            SimpleIoc.Default.Register<IProgressService, ProgressService>();
            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();

            SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformWinRT>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<INavigationService, DesignNavigationService>();

            }
            else
            {
                var navigationService = CreateNavigationService();
                SimpleIoc.Default.Register(() => navigationService);
            }


            //create global viewmodels
            SimpleIoc.Default.Register<ProgressViewModel>();
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();

            navigationService.Configure(PageKeys.Main.ToString(), typeof(MainPage));
            navigationService.Configure(PageKeys.Feed.ToString(), typeof(FeedPage));
            navigationService.Configure(PageKeys.Article.ToString(), typeof(ArticlePage));
            navigationService.Configure(PageKeys.Settings.ToString(), typeof(SettingsPage));

            return navigationService;
        }
    }
}