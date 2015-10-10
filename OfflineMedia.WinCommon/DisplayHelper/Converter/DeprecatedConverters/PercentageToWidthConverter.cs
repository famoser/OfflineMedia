using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.DeprecatedConverters
{
    public class PercentageToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var percentage = (double) value;
            var totalwidth = parameter != null ? (double) parameter : 0;

            return totalwidth*percentage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
