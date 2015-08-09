using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OfflineMediaV3.DisplayHelper.Converter.SpecificConverter
{
    public class BoolToGridLenghtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (bool)value;
            if (res)
                return new GridLength(1, GridUnitType.Star);

            return GridLength.Auto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
