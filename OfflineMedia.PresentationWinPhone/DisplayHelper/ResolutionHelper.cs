using Windows.UI.Xaml;

namespace OfflineMedia.DisplayHelper
{
    public class ResolutionHelper
    {
        public double WidthOfDevice
        {
            get { return Window.Current.Bounds.Width; }
        }
    }
}
