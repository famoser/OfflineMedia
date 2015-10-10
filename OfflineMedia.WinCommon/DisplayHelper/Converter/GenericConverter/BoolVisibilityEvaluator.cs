using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.GenericConverter
{
    public class BoolVisibilityEvaluator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (bool) value;
            if (res)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
