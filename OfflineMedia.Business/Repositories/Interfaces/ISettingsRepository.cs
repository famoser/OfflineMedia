using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Framework.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        ObservableCollection<BaseSettingModel> GetSettings();

        Task<BaseSettingModel> GetSettingByKeyAsync(SettingKey key);
        
        Task<bool> SaveSettingsAsync();

        ObservableCollection<BaseSettingModel> GetSampleSettings();
    }
}
