using Windows.UI.Xaml;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.DisplayHelper
{
    public class ResolutionHelper : SingletonBase<ResolutionHelper>
    {
        public double WidthOfDevice
        {
            get { return Window.Current.Bounds.Width; }
        }

        public double HeightOfDevice
        {
            get { return Window.Current.Bounds.Height; }
        }
    }
}
