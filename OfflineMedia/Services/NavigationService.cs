using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using Xamarin.Forms;

namespace OfflineMedia.Services
{
    public class NavigationService : INavigationService
    {
        Dictionary<string, Type> Pages { get; }
        string _currentPageKey;

        public NavigationService()
        {
            Pages = new Dictionary<string, Type>
            {
                {PageKeys.Main.ToString(), typeof (MainPage)}
            };

        }

        public Page MainPage
        {
            get
            {
                return Application.Current.MainPage;
            }
        }

        #region INavigationService implementation

        public void GoBack()
        {
            if (MainPage.Navigation.ModalStack.Count > 0)
            {
                MainPage.Navigation.PopModalAsync();
            }
            else
            {
                MainPage.Navigation.PopAsync();
            }
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            try
            {
                object[] parameters = null;
                if (parameter != null)
                {
                    parameters = new object[] { parameter };
                }
                Page displayPage = (Page)Activator.CreateInstance(Pages[pageKey], parameters);
                _currentPageKey = pageKey;
                /*
                var isModal = displayPage is IModalPage;
                if (isModal)
                {
                    MainPage.Navigation.PushModalAsync(new NavigationPage(displayPage));
                }
                else
                {*/
                    MainPage.Navigation.PushAsync(displayPage);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public string CurrentPageKey
        {
            get
            {
                return _currentPageKey;
            }
        }

        #endregion
    }
}
