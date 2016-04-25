using GalaSoft.MvvmLight.Views;

namespace OfflineMedia.Platform
{
    public static class NavigationHelper
    {
        public static INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            
            return navigationService;
        }
    }
}
