using System;
using GalaSoft.MvvmLight;
using OfflineMediaV3.Business.Enums;

namespace OfflineMediaV3.Business.Models.Configuration
{
    public class SettingModel : BaseModel
    {
        public Guid Guid { get; set; }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public string DefaultValue { get; set; }

        public SettingKeys Key { get; set; }
        public ValueTypeEnum ValueType { get; set; }
        public bool IsChangeable { get; set; }
        public bool IsCriticalChange { get; set; }

        public string Description { get; set; }

        /* FOR TYPE: PossibleValues */
        public string[] PossibleValues { get; set; }

        /* FOR TYPE: TrueOrFalse */
        public string OnContent { get; set; }
        public string OffContent { get; set; }
        public bool IsOn
        {
            get
            {
                return Value == "True";
            }
            set
            {
                Value = value.ToString();
            }
        }


        /* FOR TYPE: int */
        public int IntValue
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Value);
                }
                catch
                {
                    Value = "0";
                    return IntValue;
                }
            }
            set
            {
                Value = value.ToString();
            }
        }
    }
}
