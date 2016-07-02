using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticleListConverter
{
    public class IsReadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = (bool)value;
            if (dt)
                return 0.8;

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
