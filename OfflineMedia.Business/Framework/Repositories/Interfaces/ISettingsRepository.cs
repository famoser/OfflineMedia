using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<List<SourceConfigurationModel>> GetSourceConfigurations();

        Task<List<SettingModel>> GetAllSettings();

        List<SettingModel> GetSampleSettings();

        List<SourceConfigurationModel> GetSampleSourceConfiguration();
        
        Task<SettingModel> GetSettingByKey(SettingKey key);

        Task<bool> SaveSettings();
        Task<bool> SaveSetting(SimpleSettingModel ssm);
        Task<bool> SaveSettingByKey(SettingKey key, string value);

        Task<FeedConfigurationModel> GetFeedConfigurationFor(Guid guid);
    }
}
