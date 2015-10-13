using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.DebugConverters
{
    public class PassAll : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                

                //ht breakpoint
                "hallo".ToString();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
