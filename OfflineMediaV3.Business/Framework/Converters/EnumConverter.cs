using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Framework.Converters
{
    public class EnumConverter<T> : IEntityValueConverter
    {
        public object Convert(object val)
        {
            return (T)val;
        }

        public object ConvertBack(object val)
        {
            return (int)val;
        }
    }
}
