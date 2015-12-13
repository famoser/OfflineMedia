using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.GenericConverter
{
    public class ListVisibillityEvaluator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as IList;
            if (str?.Count > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
