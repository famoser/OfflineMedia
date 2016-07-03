using System;
using Famoser.OfflineMedia.Data.Enums;

namespace Famoser.OfflineMedia.Data.Entities.Storage.Settings
{
    public class SettingEntity
    {
        public Guid Guid { get; set; }
        public SettingKey SettingKey { get; set; }
        public string Value { get; set; }

        public SettingValueType SettingValueType { get; set; }

        public string Name { get; set; }
        public bool IsImmutable { get; set; }

        /* FOR TYPE: TrueOrFalse */
        public string OnContent { get; set; }
        public string OffContent { get; set; }

        /* FOR TYPE: TrueOrFalse */
        public string[] PossibleValues { get; set; }
    }
}
