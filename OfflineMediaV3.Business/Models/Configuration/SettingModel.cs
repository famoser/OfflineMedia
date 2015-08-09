using System;
using OfflineMediaV3.Business.Enums.Settings;

namespace OfflineMediaV3.Business.Models.Configuration
{
    public class SettingModel : SimpleSettingModel
    {
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
    }
}
