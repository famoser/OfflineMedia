using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OfflineMediaV3.Business.Enums.Generic;
using OfflineMediaV3.Business.Framework;

namespace OfflineMediaV3.Business.Models.Configuration
{
    public class GuidSettingModel : BaseModel
    {
        [EntityMap]
        [EntityConversion(typeof(string), typeof(Guid))]
        public Guid Guid { get; set; }

        private bool _isEnabled;
        [EntityMap(EntityMappingProcedure.ReadAndWrite, "Value")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { Set(ref _isEnabled, value); }
        }
    }
}
