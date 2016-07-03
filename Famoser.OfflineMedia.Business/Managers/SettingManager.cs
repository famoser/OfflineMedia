using System.Collections.Generic;
using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Helpers;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Data.Entities.Storage.Settings;

namespace Famoser.OfflineMedia.Business.Managers
{
    public class SettingManager
    {
        private static readonly ObservableCollection<BaseSettingModel> AllSettings = new ObservableCollection<BaseSettingModel>();
        private static readonly ObservableCollection<BaseSettingModel> EditableSettings = new ObservableCollection<BaseSettingModel>();

        public static void AddSetting(BaseSettingModel model)
        {
            AllSettings.Add(model);
            if (!model.IsImmutable)
                EditableSettings.Add(model);
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

        public static ObservableCollection<BaseSettingModel> GetEditableSettings()
        {
            return EditableSettings;
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
