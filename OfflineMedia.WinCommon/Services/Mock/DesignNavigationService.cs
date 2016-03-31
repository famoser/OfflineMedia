using System;
using GalaSoft.MvvmLight.Views;

namespace OfflineMedia.Services.Mock
{
    public class DesignNavigationService : INavigationService
    {
        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            throw new NotImplementedException();
        }

        public string CurrentPageKey
        {
            get { throw new NotImplementedException(); }
        }
    }
}
