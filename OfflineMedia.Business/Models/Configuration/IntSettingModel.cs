using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.Configuration.Base;

namespace OfflineMedia.Business.Models.Configuration
{
    public class IntSettingModel : BaseSettingModel
    {
        public int IntValue { get { return Convert.ToInt32(Value); } set { Value = value.ToString(); } }
    }
}
