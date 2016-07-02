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

using Famoser.OfflineMedia.Business.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using OfflineMedia.Platform;
using OfflineMedia.Services;
using OfflineMedia.Services.Mock;
using OfflineMedia.View.ViewModels;
using OfflineMedia.WinCommon.Services;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace OfflineMedia.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public partial class ViewModelLocator : BaseViewModelLocator
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
            SimpleIoc.Default.Register<IVariaService, VariaService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IApiService, ApiService>();
            SimpleIoc.Default.Register<IPlatformCodeService, PlatformCodeService>();

            SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformWinRT>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<INavigationService, DesignNavigationService>();
                SimpleIoc.Default.Register<IDispatchService, DesignDispatchService>();
            }
            else
            {
                var navigationService = NavigationHelper.CreateNavigationService();
                SimpleIoc.Default.Register(() => navigationService);
                SimpleIoc.Default.Register<IDispatchService, DispatchWinRtService>();
            }
        }
    }
}