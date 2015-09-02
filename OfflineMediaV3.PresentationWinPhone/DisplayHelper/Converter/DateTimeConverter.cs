using System;
using Windows.UI.Xaml.Data;

namespace OfflineMediaV3.DisplayHelper.Converter
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            if (dt == DateTime.MinValue)
                return "unbekanntes Veröffentlichungsdatum";
            return dt.ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

}
