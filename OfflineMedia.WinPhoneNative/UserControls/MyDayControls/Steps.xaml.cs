using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.MyDayHelpers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OfflineMedia.UserControls.MyDayControls
{
    public sealed partial class Steps : UserControl
    {
        public Steps()
        {
            this.InitializeComponent();
            Init();
        }

        public async void Init()
        {
            try
            {
                var sp = new StepHelper();
                WalkingSteps.Text = (await sp.GetStepsToday()).ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Warning, this, "Steps could not initialize", ex);
                ContainingGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
