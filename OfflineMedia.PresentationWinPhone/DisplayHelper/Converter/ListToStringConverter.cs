using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var arr = value as List<string>;
            if (arr != null && arr.Any())
            {
                var res = arr[0];
                for (int i = 1; i < arr.Count; i++)
                {
                    res += ", " + arr[i];
                }
                return res;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
