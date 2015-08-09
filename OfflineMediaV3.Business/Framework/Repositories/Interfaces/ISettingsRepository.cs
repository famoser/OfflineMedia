using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.Business.Framework.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        Task<List<SourceConfigurationModel>> GetSourceConfigurations();

        Task<List<SettingModel>> GetAllSettings();

        List<SettingModel> GetSampleSettings();

        List<SourceConfigurationModel> GetSampleSourceConfiguration();


        Task<SettingModel> GetSettingByKey(SettingKeys key);

        Task<bool> SaveSettings();
    }
}
