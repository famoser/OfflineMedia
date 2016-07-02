using System;
using Windows.UI.Xaml.Data;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.MyDayConverter
{
    public class FullDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = (DateTime)value;
            if (dt.Date == DateTime.Today)
                return "heute " + dt.ToString("HH:mm");
            if (dt.Date.Subtract(TimeSpan.FromDays(1)) == DateTime.Today)
                return "morgen " + dt.ToString("HH:mm");
            if (dt.Date.AddDays(1) == DateTime.Today)
                return "gestern " + dt.ToString("HH:mm");
            return dt.ToString("dd.MM.yyyy HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }

}
