using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.GenericConverter
{
    public class ReferenceVisibillityEvaluator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    if (value is string)
                    {
                        var str = value as string;
                        if (string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(str))
                            return Visibility.Collapsed;
                    }
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
