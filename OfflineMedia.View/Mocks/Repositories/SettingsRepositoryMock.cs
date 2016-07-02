using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Enums;

namespace Famoser.OfflineMedia.View.Mocks.Repositories
{
    class SettingsRepositoryMock: ISettingsRepository
    {
        public ObservableCollection<BaseSettingModel> GetSettings()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<BaseSettingModel> GetEditSettings()
        {
            throw new NotImplementedException();
        }

        public Task<BaseSettingModel> GetSettingByKeyAsync(SettingKey key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<BaseSettingModel> GetSampleSettings()
        {
            throw new NotImplementedException();
        }
    }
}
