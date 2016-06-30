using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Models.Configuration
{
    public class SelectSettingModel : BaseSettingModel
    {
        public string[] PossibleValues { get; set; }
    }
}
