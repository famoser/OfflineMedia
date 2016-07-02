using System;
using Windows.UI.Xaml.Data;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticleListConverter
{
    public class DetailedDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            if (dt < DateTime.MinValue + TimeSpan.FromDays(1))
                return "";
            return dt.ToString("dd.MM.yyyy HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

}
