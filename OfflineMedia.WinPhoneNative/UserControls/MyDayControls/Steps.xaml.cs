using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Famoser.FrameworkEssentials.Logging;
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
                LogHelper.Instance.Log(LogLevel.Warning, "Steps could not initialize", this, ex);
                ContainingGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
