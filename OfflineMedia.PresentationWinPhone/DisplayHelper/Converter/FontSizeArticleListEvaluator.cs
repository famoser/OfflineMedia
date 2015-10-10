using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter
{
    public class FontSizeArticleListEvaluator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
                return 11;
            else
                return 16;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
