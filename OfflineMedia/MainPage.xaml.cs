using OfflineMedia.View.ViewModels;
using Xamarin.Forms;

namespace OfflineMedia
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
			BindingContext = BaseViewModelLocator.Instance.MainPageViewModel;
        }
    }
}
