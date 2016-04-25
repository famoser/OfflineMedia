using System;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.Configuration
{
    public class SimpleSettingModel : BaseModel
    {
        [EntityMap]
        [EntityConversion(typeof(string), typeof(Guid))]
        public Guid Guid { get; set; }

        private string _value;
        [EntityMap]
        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        private bool _boolValue;
        public bool BoolValue
        {
            get
            {
                _boolValue = Convert.ToBoolean(Value);
                return _boolValue;
            }
            set
            {
                Value = value.ToString();
                Set(ref _boolValue, value);
            }
        }

        private int _intValue;
        public int IntValue
        {
            get
            {
                _intValue =Convert.ToInt32(Value);
                return _intValue;
            }
            set
            {
                Value = value.ToString();
                Set(ref _intValue, value);
            }
        }
    }
}
