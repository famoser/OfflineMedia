using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.SqliteWrapper.Repositories;
using Newtonsoft.Json;
using Nito.AsyncEx;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Managers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Repositories.Base;
using OfflineMedia.Business.Services;
using OfflineMedia.Data.Entities;
using OfflineMedia.Data.Entities.Storage;
using OfflineMedia.Data.Entities.Storage.Settings;
using OfflineMedia.Data.Enums;
using ValueType = OfflineMedia.Business.Enums.Settings.ValueType;

namespace OfflineMedia.Business.Framework.Repositories
{
    public class SettingsRepository : BaseRepository, ISettingsRepository
    {
        private readonly IStorageService _storageService;

        public SettingsRepository(IStorageService storageService)
        {
            _storageService = storageService;
        }

        #region sample

        #endregion

        public ObservableCollection<BaseSettingModel> GetSettings()
        {
#pragma warning disable 4014
            Initialize();
#pragma warning restore 4014
            return SettingManager.GetAllSettings();
        }

        private bool _isInitialized = false;
        private readonly AsyncLock _initializeAsyncLock = new AsyncLock();
        private async Task Initialize()
        {
            using (await _initializeAsyncLock.LockAsync())
            {
                if (_isInitialized)
                    return;

                var jsonAssets = await _storageService.GetAssetTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.SettingsConfiguration).Description);
                var defaultSettings = JsonConvert.DeserializeObject<List<SettingEntity>>(jsonAssets);

                SettingCacheEntity cache = new SettingCacheEntity();
                try
                {
                    var json = await _storageService.GetCachedTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.UserConfiguration).Description);

                    if (!string.IsNullOrEmpty(json))
                    {
                        cache = JsonConvert.DeserializeObject<SettingCacheEntity>(json);
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
            }
        }


        public Task<BaseSettingModel> GetSettingByKeyAsync(SettingKey key)
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();
                return SettingManager.GetAllSettings().FirstOrDefault(s => s.SettingKey == key);
            });
        }

        public Task<bool> SaveSettingsAsync()
        {
            return ExecuteSafe(async () =>
            {
                var json = JsonConvert.SerializeObject(SettingManager.GetSettingCache());
                return await _storageService.SetCachedTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.UserConfiguration).Description, json);
            });
        }

        public Task<bool> SetFeedActiveStateAsync(FeedModel feedModel, bool isActive)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SetSourceActiveStateAsync(SourceModel sourceModel, bool isActive)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<BaseSettingModel> GetSampleSettings()
        {
            return new ObservableCollection<BaseSettingModel>()
            {
                new IntSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.FontSize,
                    Name = "Int Setting",
                    IntValue = 3,
                },
                new SelectSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.Font,
                    Name = "Select Font",
                    Value = "Times New Roman",
                    PossibleValues = new [] {"Times New Roman", "Segoe UI"}
                },
                new  TextSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.WeatherCities,
                    Name = "Text Setting",
                    Value = "Basel, Zürich"
                },
                new TrueOrFalseSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.DisplayFavorites,
                    Name = "True or False Setting",
                    BoolValue = true,
                    OnContent = "On Content",
                    OffContent = "Off Content"
                },
            };
        }
    }
}
