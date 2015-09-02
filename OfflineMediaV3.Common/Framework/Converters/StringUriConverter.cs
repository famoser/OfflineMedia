using System;

namespace OfflineMediaV3.Common.Framework.Converters
{
    public class StringUriConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            var str = val as string;
            if (val != null)
                return new Uri(str);
            return null;
        }

        public object ConvertBack(object val)
        {
            var uri = val as Uri;
            if (uri != null)
                return uri.ToString();
            return null;
        }
    }
}
