using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.WinUniversal.DisplayHelper.Converter.GenericConverter
{
    public class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var arr = value as string[];
            if (arr != null && arr.Any())
            {
                return string.Join(", ", arr);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
