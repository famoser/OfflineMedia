using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Entities
{
    public class GuidSettingEntity : EntityBase
    {
        public string Guid { get; set; }
        public bool Value { get; set; }
    }
}
