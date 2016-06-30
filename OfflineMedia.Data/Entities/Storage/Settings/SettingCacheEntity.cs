using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Data.Entities.Storage.Settings
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
