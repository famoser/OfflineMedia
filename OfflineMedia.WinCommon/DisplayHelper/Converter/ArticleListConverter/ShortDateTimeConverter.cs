using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.ArticleListConverter
{
    public class ShortDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            if (dt == DateTime.MinValue)
                return "unbekannt";
            return dt.ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
