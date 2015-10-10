using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter
{
    public class HeightEvaluator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? b = value as bool?;
            if (b.HasValue && b.Value)
                return double.NaN;
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
