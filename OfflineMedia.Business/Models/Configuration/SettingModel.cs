using System;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.Configuration
{
    public class SettingModel : BaseModel
    {
        public Guid Guid { get; set; }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (Set(ref _value, value))
                {
                    RaisePropertyChanged(() => IntValue);
                    RaisePropertyChanged(() => BoolValue);
                }
            }
        }

        public bool BoolValue { get { return Convert.ToBoolean(Value); } set { Value = value.ToString(); } }
        public int IntValue { get { return Convert.ToInt32(Value); } set { Value = value.ToString(); } }
    }
}
