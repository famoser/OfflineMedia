using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OfflineMediaV3.DisplayHelper.Converter
{
    public class IntToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int val = (int) value;
            int param = int.Parse((string)parameter);
            return val + param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
