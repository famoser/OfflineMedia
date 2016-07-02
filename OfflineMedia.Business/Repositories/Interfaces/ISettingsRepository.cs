using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.Configuration.Base;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Repositories.Interfaces
{
    public interface ISettingsRepository
    {
        ObservableCollection<BaseSettingModel> GetSettings();
        ObservableCollection<BaseSettingModel> GetEditSettings();

        Task<BaseSettingModel> GetSettingByKeyAsync(SettingKey key);
        
        Task<bool> SaveSettingsAsync();

        ObservableCollection<BaseSettingModel> GetSampleSettings();
    }
}
