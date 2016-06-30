using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Models.Configuration
{
    public class TrueOrFalseSettingModel : BaseSettingModel
    {
        public bool BoolValue
        {
            get { return Convert.ToBoolean(Value); }
            set
            {
                Value = value.ToString();
                RaisePropertyChanged(() => BoolValue);
            }
        }

        public string OnContent { get; set; }
        public string OffContent { get; set; }
    }
}
