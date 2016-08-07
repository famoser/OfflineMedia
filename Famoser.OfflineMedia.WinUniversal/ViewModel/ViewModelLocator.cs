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

using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.View.ViewModels;
using Famoser.OfflineMedia.WinUniversal.Platform;
using Famoser.OfflineMedia.WinUniversal.Platform.Mock;
using Famoser.SqliteWrapper.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace Famoser.OfflineMedia.WinUniversal.ViewModel
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
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Create design time view services and models
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IPlatformCodeService, PlatformCodeService>();
            SimpleIoc.Default.Register<IStorageService>(() => new StorageService());
            
            SimpleIoc.Default.Register<ISQLitePlatform, SQLitePlatformWinRT>();
            SimpleIoc.Default.Register<ISqliteServiceSettingsProvider, SqliteServiceSettingsProvider>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IHistoryNavigationService, HistoryNavigationServiceMock>();
            }
            else
            {
                SimpleIoc.Default.Register<IHistoryNavigationService>(NavigationHelper.CreateNavigationService);
            }
        }
    }
}