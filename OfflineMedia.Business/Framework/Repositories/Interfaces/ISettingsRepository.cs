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
        ObservableCollection<SettingModel> GetSettings();

        Task<SettingModel> GetSettingByKeyAsync(SettingKey key);
        
        Task<bool> SaveSettingAsync(SettingKey ssm);
        Task<bool> SetFeedActiveStateAsync(FeedModel feedModel, bool isActive);
        Task<bool> SetSourceActiveStateAsync(SourceModel sourceModel, bool isActive);

        ObservableCollection<SettingModel> GetSampleSettings();
    }
}
