using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Entities
{
    public class SettingEntity : EntityBase
    {
        public int SettingKey { get; set; }
        public string Value { get; set; }
    }
}
