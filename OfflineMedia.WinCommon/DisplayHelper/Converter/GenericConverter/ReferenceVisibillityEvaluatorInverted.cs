using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.GenericConverter
{
    public class ReferenceVisibillityEvaluatorInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var str = value as string;
                if (str != null && (string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(str)))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
