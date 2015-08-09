using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Common.Framework.Converters
{
    public class StringBoolConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            var str = val as string;
            return System.Convert.ToBoolean(str);
        }

        public object ConvertBack(object val)
        {
            var bolean = (bool)val;
            return bolean.ToString();
        }
    }
}
