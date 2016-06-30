using OfflineMedia.Business.Models.Base;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Models.Configuration.Base
{
    public abstract class BaseSettingModel : BaseGuidModel
    {
        public string Name { get; set; }
        public SettingKey SettingKey { get; set; }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }
    }
}
