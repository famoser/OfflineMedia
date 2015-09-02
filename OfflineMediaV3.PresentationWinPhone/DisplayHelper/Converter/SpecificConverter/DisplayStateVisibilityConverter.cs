using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using OfflineMediaV3.Common.Enums.View;

namespace OfflineMediaV3.DisplayHelper.Converter.SpecificConverter
{
    public class DisplayStateVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DisplayState ds = (DisplayState)value;
            if (ds.ToString() == (string)parameter)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
