using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using OfflineMediaV3.View.ViewModels;

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
