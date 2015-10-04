using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OfflineMediaV3.DisplayHelper.Converter.SpecificConverter
{
    public class HoverDivStoryboardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (double) value - 108;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
