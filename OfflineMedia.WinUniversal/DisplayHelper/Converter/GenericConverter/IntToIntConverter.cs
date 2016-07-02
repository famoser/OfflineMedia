using System;
using Windows.UI.Xaml.Data;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.GenericConverter
{
    public class IntToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int val = (int) value;
            int param = int.Parse((string)parameter);
            return val + param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
