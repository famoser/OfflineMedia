using System;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Data.Enums;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.Configuration
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
