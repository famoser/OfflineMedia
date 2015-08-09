using System;

namespace OfflineMediaV3.Common.Framework.Converters
{
    public class StringUriConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            return new Uri((string)val);
        }

        public object ConvertBack(object val)
        {
            var uri = (Uri)val;
            return uri.ToString();
        }
    }
}
