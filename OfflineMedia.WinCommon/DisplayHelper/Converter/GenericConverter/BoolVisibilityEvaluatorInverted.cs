using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.GenericConverter
{
    public class BoolVisibilityEvaluatorInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (bool)value;
            if (res)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
