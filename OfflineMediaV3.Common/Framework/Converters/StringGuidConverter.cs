using System;

namespace OfflineMediaV3.Common.Framework.Converters
{
    public class StringGuidConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            var str = val as string;
            return Guid.Parse(str);
        }

        public object ConvertBack(object val)
        {
            var guid = (Guid) val;
            return guid.ToString();
        }
    }
}
