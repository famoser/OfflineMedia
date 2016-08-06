using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Famoser.OfflineMedia.View.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Famoser.OfflineMedia.WinUniversal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedPage : Page
    {
        public FeedPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        public FeedPageViewModel ViewModel => DataContext as FeedPageViewModel;
        private bool _propertyChangedRegistered;

        private void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!_propertyChangedRegistered)
            {
                ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
                _propertyChangedRegistered = true;
            }

        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Feed")
            {
                FeedScrollViewer.ChangeView(0, 0, 1); //this does not work all the time
                FeedScrollViewer.ScrollToVerticalOffset(0);
            }
        }
    }
}
