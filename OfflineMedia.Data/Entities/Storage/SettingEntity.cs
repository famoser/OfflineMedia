using System;
using OfflineMedia.Business.Enums.Settings;
using ValueType = OfflineMedia.Business.Enums.Settings.ValueType;

namespace OfflineMedia.Business.Models.Configuration
{
    public class SettingEntity
    {
        public Guid Guid { get; set; }
        public SettingKeys SettingKey { get; set; }
        public string Value { get; set; }

        public ValueType ValueType { get; set; }

        public string Name { get; set; }

        /* FOR TYPE: TrueOrFalse */
        public string OnContent { get; set; }
        public string OffContent { get; set; }

        /* FOR TYPE: TrueOrFalse */
        public string[] PossibleValues { get; set; }
    }
}
