using System;
using Windows.UI.Xaml.Data;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.DateTime
{
    class DateTimeToDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            System.DateTime dt = (System.DateTime)value;
            if (dt.Date == System.DateTime.Today)
                return "heute";
            if (dt.Date.Subtract(TimeSpan.FromDays(1)) == System.DateTime.Today)
                return "morgen";
            if (dt.Date.AddDays(1) == System.DateTime.Today)
                return "gestern";
            return dt.ToString("dd. MM.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
