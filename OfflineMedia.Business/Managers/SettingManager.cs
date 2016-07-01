using System.Collections.Generic;
using System.Collections.ObjectModel;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.Configuration.Base;
using OfflineMedia.Data.Entities.Storage;
using OfflineMedia.Data.Entities.Storage.Settings;

namespace OfflineMedia.Business.Managers
{
    public class SettingManager
    {
        private static readonly ObservableCollection<BaseSettingModel> AllSettings = new ObservableCollection<BaseSettingModel>();

        public static void AddSetting(BaseSettingModel model)
        {
            AllSettings.Add(model);
        }

        public static void AddSetting(SettingEntity entity)
        {
            var model = EntityModelConverter.Convert(entity);
            AddSetting(model);
        }

        public static void AddAllSettings(IEnumerable<BaseSettingModel> models)
        {
            foreach (var sourceModel in models)
            {
                AddSetting(sourceModel);
            }
        }

        public static ObservableCollection<BaseSettingModel> GetAllSettings()
        {
            return AllSettings;
        }

        public static SettingCacheEntity GetSettingCache()
        {
            var cache = new SettingCacheEntity();
            foreach (var baseSettingModel in AllSettings)
            {
                cache.SettingCacheItemEntities.Add(new SettingCacheItemEntity()
                {
                    Guid = baseSettingModel.Guid,
                    Value = baseSettingModel.Value
                });
            }
            return cache;
        }
    }
}
