using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Enums;
using Famoser.OfflineMedia.Business.Managers;
using Famoser.OfflineMedia.Business.Models.Configuration;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Repositories.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Entities.Storage.Settings;
using Famoser.OfflineMedia.Data.Enums;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.Business.Repositories
{
    public class SettingsRepository : BaseRepository, ISettingsRepository
    {
        private readonly IStorageService _storageService;

#pragma warning disable 4014
        public SettingsRepository(IStorageService storageService)
        {
            _storageService = storageService;
            Initialize();
        }

        #region sample

        #endregion

        public ObservableCollection<BaseSettingModel> GetSettings()
        {
            Initialize();
            return SettingManager.GetAllSettings();
        }

        public ObservableCollection<BaseSettingModel> GetEditSettings()
        {
            Initialize();
            return SettingManager.GetEditableSettings();
        }
#pragma warning restore 4014

        private bool _isInitialized;
        private readonly AsyncLock _initializeAsyncLock = new AsyncLock();
        private Task Initialize()
        {
            return ExecuteSafe(async () =>
            {
                using (await _initializeAsyncLock.LockAsync())
                {
                    if (_isInitialized)
                        return;

                    var jsonAssets =
                        await
                            _storageService.GetAssetTextFileAsync(
                                ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                    FileKeys.SettingsConfiguration).Description);
                    var defaultSettings = JsonConvert.DeserializeObject<List<SettingEntity>>(jsonAssets);
                    var recovered = false;

                    SettingCacheEntity cache = new SettingCacheEntity();
                    try
                    {
                        var json =
                            await
                                _storageService.GetCachedTextFileAsync(
                                    ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                        FileKeys.SettingsUserConfiguration).Description);

                        if (!string.IsNullOrEmpty(json))
                        {
                            cache = JsonConvert.DeserializeObject<SettingCacheEntity>(json);
                            recovered = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogException(ex);
                    }

                    foreach (var defaultSetting in defaultSettings)
                    {
                        var savedSetting = cache.SettingCacheItemEntities.FirstOrDefault(s => s.Guid == defaultSetting.Guid);
                        if (savedSetting != null && !defaultSetting.IsImmutable)
                            defaultSetting.Value = savedSetting.Value;
                        SettingManager.AddSetting(defaultSetting);
                    }

                    _isInitialized = true;

                    //do saving fire&forget to free lockings
#pragma warning disable 4014
                    if (!recovered)
                        SaveSettingsAsync();
#pragma warning restore 4014
                }
            });
        }


        public async Task<BaseSettingModel> GetSettingByKeyAsync(SettingKey key)
        {
            await Initialize();
            return SettingManager.GetAllSettings().FirstOrDefault(s => s.SettingKey == key);
        }

        public Task<bool> SaveSettingsAsync()
        {
            return ExecuteSafe(async () =>
            {
                var json = JsonConvert.SerializeObject(SettingManager.GetSettingCache());
                return await _storageService.SetCachedTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SettingsUserConfiguration).Description, json);
            });
        }
    }
}
