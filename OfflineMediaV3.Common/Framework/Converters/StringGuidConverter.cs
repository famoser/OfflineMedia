using System;

namespace OfflineMediaV3.Common.Framework.Converters
{
    public class StringGuidConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            var str = val as string;
            if (str != null)
                return Guid.Parse(str);
            return Guid.Empty;
        }

        public object ConvertBack(object val)
        {
            var guid = (Guid)val;
            return guid.ToString();
        }
    }
}
