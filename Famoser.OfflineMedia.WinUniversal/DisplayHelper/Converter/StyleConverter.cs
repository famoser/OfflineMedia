using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Famoser.FrameworkEssentials.UniversalWindows.Helpers;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter
{
    public class StyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var styles = ((string)parameter).Split(new[] { "|" }, StringSplitOptions.None);
            if (ResolutionHelper.WidthOfDevice > 1200 && styles.Length > 2)
                return Application.Current.Resources[styles[2]];
            if (ResolutionHelper.WidthOfDevice > 800 && styles.Length > 1)
                return Application.Current.Resources[styles[1]];
            if (styles.Length > 0)
                return Application.Current.Resources[styles[0]];
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
