using System;
using Windows.UI.Xaml.Data;
using OfflineMediaV3.Business.Enums.Models;

namespace OfflineMediaV3.DisplayHelper.Converter
{
    public class IsReadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = (ArticleState)value;
            if (dt == ArticleState.Read)
                return 0.6;

                return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
