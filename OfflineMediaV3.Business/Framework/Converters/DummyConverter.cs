using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Framework.Converters
{
    public class DummyConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object val)
        {
            throw new NotImplementedException();
        }
    }
}
