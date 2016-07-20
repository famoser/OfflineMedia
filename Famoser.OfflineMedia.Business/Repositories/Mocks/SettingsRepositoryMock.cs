using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.Configuration;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Enums;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.Business.Repositories.Mocks
{
    public class SettingsRepositoryMock : ISettingsRepository
    {
        private readonly ObservableCollection<BaseSettingModel> _sampleSettings = new ObservableCollection<BaseSettingModel>()  {
                new IntSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.FontSize,
                    Name = "FontSize Setting",
                    IntValue = 3,
                },
                new IntSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.ConcurrentThreads,
                    Name = "ConcurrentThreads Setting",
                    IntValue = 5,
                },
                new IntSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.WordsPerMinute,
                    Name = "WordsPerMinute Setting",
                    IntValue = 5,
                },
                new IntSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.FastLoadedFeedEntries,
                    Name = "FastLoadedFeedEntries Setting",
                    IntValue = 5,
                },
                new SelectSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.Font,
                    Name = "Font Setting",
                    Value = "Times New Roman",
                    PossibleValues = new [] {"Times New Roman", "Segoe UI"}
                },
                new  TextSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.WeatherCities,
                    Name = "WeatherCities Setting",
                    Value = "Basel, Zürich"
                },
                new TrueOrFalseSettingModel()
                {
                    Guid = Guid.NewGuid(),
                    SettingKey = SettingKey.DisplayFavorites,
                    Name = "DisplayFavorites Setting",
                    BoolValue = true,
                    OnContent = "On Content",
                    OffContent = "Off Content"
                },
            };
        public ObservableCollection<BaseSettingModel> GetSettings()
        {
            return _sampleSettings;
        }

        public ObservableCollection<BaseSettingModel> GetEditSettings()
        {
            return _sampleSettings;
        }
        public async Task<BaseSettingModel> GetSettingByKeyAsync(SettingKey key)
        {
            return _sampleSettings.FirstOrDefault(s => s.SettingKey == key);
        }

        public async Task<bool> SaveSettingsAsync()
        {
            return true;
        }

        public async Task ResetApplicationAsync()
        {

        }
    }
}
