using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Data.Enums;

namespace Famoser.OfflineMedia.Business.Repositories.Interfaces
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
