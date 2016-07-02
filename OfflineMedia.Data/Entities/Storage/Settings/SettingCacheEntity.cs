using System.Collections.Generic;

namespace Famoser.OfflineMedia.Data.Entities.Storage.Settings
{
    public class SettingCacheEntity
    {
        public SettingCacheEntity()
        {
            SettingCacheItemEntities = new List<SettingCacheItemEntity>();
        }
        public List<SettingCacheItemEntity> SettingCacheItemEntities { get; set; }
    }
}
