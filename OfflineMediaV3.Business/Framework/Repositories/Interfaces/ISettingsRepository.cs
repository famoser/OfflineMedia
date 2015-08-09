using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Settings;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.Data;

namespace OfflineMediaV3.Business.Framework.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<List<SourceConfigurationModel>> GetSourceConfigurations(IDataService dataService);

        Task<List<SettingModel>> GetAllSettings(IDataService dataService);

        List<SettingModel> GetSampleSettings();

        List<SourceConfigurationModel> GetSampleSourceConfiguration();


        Task<SettingModel> GetSettingByKey(SettingKeys key, IDataService dataService);

        Task<bool> SaveSettings();

        Task<SourceConfigurationModel> GetSourceConfigurationFor(Guid guid, IDataService dataService);
    }
}
