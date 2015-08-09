using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Framework.Converters
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
