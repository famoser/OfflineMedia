using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.MyDayConverter
{
    class DateToDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            if (dt.Date == DateTime.Today)
                return "heute";
            if (dt.Date.Subtract(TimeSpan.FromDays(1)) == DateTime.Today)
                return "morgen";
            if (dt.Date.AddDays(1) == DateTime.Today)
                return "gestern";
            return dt.ToString("dd. MM.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
