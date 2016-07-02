using System;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;

namespace Famoser.OfflineMedia.Business.Models.Configuration
{
    public class IntSettingModel : BaseSettingModel
    {
        public int IntValue { get { return Convert.ToInt32(Value); } set { Value = value.ToString(); } }
    }
}
