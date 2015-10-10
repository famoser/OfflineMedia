using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.DisplayHelper.Converter.SpecificConverter
{
    public class ULongToByteConverter : IValueConverter
    {
        private const ulong OneKiloByte = 1024;
        private const ulong OneMegaByte = OneKiloByte * 1024;
        private const ulong OneGigaByte = OneMegaByte * 1024;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ulong size = (ulong) value;

            string suffix;
            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = "GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = "MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = "kB";
            }
            else
            {
                suffix = " B";
            }

            return size + suffix;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
