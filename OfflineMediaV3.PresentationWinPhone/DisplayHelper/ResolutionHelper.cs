using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OfflineMediaV3.DisplayHelper
{
    public class ResolutionHelper
    {
        public double WidthOfDevice
        {
            get { return Window.Current.Bounds.Width; }
        }
    }
}
