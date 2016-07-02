using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.WinUniversal.DisplayHelper.Converter.MyDayConverter
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            return dt.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

}
