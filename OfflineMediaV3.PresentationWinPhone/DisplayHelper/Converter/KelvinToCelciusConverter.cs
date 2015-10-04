using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OfflineMediaV3.DisplayHelper.Converter
{
    public class KelvinToCelciusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double val = (double) value;
            return Math.Round(val - 273.15,0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
