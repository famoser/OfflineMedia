using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Common.Framework;

namespace OfflineMediaV3.Business.Models.Configuration
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
            get { return Convert.ToBoolean(Value); }
            set
            {
                if (Set(ref _boolValue, value))
                    Value = value.ToString();
            }
        }

        private int _intValue;
        public int IntValue
        {
            get { return Convert.ToInt32(Value); }
            set
            {
                if (Set(ref _intValue, value))
                    Value = value.ToString();
            }
        }
    }
}
