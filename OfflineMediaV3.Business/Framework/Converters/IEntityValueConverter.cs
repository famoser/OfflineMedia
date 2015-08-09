using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Framework.Converters
{
    public interface IEntityValueConverter
    {
        object Convert(object val);

        object ConvertBack(object val);
    }
}
