using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Managers;
using OfflineMedia.Business.Models.WeatherModel;
using OfflineMedia.Business.Newspapers.OpenWeatherMap;
using OfflineMedia.Business.Repositories.Base;
using OfflineMedia.Business.Repositories.Interfaces;
using OfflineMedia.Data.Entities.Storage;
using OfflineMedia.Data.Entities.Storage.Settings;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Repositories
{
    public class WeatherRepository : BaseRepository, IWeatherRepository
    {
        private IStorageService _storageService;
        private ISettingsRepository _settingsRepository;
        private Dictionary<string, string> _weatherFontMapping;
        private static string _apiUrl = "http://api.openweathermap.org/data/2.5/forecast?appid=3b2b694b8ac5add8b400dc24e563fd50&q={city}&lang=de";

        public WeatherRepository(IStorageService storageService, ISettingsRepository settingsRepository)
        {
            _storageService = storageService;
            _settingsRepository = settingsRepository;
        }

        private Uri GetApiUrl(Forecast forecast)
        {
            return new Uri(Uri.EscapeUriString(_apiUrl.Replace("{city}", forecast.City)));
        }

        public ObservableCollection<Forecast> GetForecasts()
        {
            Initialize();
            return ForecastManager.GetForecasts();
        }

        public ObservableCollection<Forecast> GetSampleForecasts()
        {
            var forecasts = new ObservableCollection<Forecast>();
            for (int i = 0; i < 3; i++)
            {
                var forecast = new Forecast()
                {
                    City = "City " + i
                };

                for (int j = 0; j < 3; j++)
                {
                    forecast.Forecasts.Add(new ForecastItem()
                    {
                        Description = "ein bisschen wolkig",
                        ConditionId = 801,
                        ConditionFontIcon =
                            ((char)int.Parse("EB48", System.Globalization.NumberStyles.HexNumber)).ToString(),
                        CloudinessPercentage = 80,
                        HumidityPercentage = 16,
                        WindDegreee = 310,
                        WindSpeed = 12,
                        TemperatureKelvin = 287,
                        PressurehPa = 1300,
                        Date = DateTime.Now,
                        RainVolume = 0,
                        SnowVolume = 0
                    });
                }

                forecasts.Add(forecast);
            }

            return forecasts;
        }

        private bool _isInitialized;
        private readonly AsyncLock _initializeAsyncLock = new AsyncLock();
        private Task Initialize()
        {
            return ExecuteSafe(async () =>
            {
                using (await _initializeAsyncLock.LockAsync())
                {
                    if (_isInitialized)
                        return;

                    var fontJson = await _storageService.GetAssetTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.WeatherFontInformations).Description);
                    _weatherFontMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(fontJson);

                    WeatherCacheModel cache = new WeatherCacheModel();

                    await ExecuteSafe(async () =>
                    {
                        var json = await _storageService.GetCachedTextFileAsync(
                                ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                    FileKeys.WeatherCache).Description);

                        if (!string.IsNullOrEmpty(json))
                            cache = JsonConvert.DeserializeObject<WeatherCacheModel>(json);
                    });

                    var cities = await GetCities();
                    foreach (var city in cities)
                    {
                        var oldOne = cache.Forecasts.First(f => f.City == city);
                        if (oldOne != null)
                            ForecastManager.AddForecast(oldOne);
                        else
                            ForecastManager.AddForecast(new Forecast()
                            {
                                City = city
                            });
                    }

                    _isInitialized = true;
                }
            });
        }

        private async Task<string[]> GetCities()
        {
            var cities = await _settingsRepository.GetSettingByKeyAsync(SettingKey.WeatherCities);
            if (string.IsNullOrWhiteSpace(cities.Value))
                return null;

            if (cities.Value.Contains(", "))
            {
                var localVal = cities.Value.Trim();
                while (localVal.Contains("  "))
                {
                    localVal = localVal.Replace("  ", " ");
                }
                return localVal.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            }
            return new[] { cities.Value.Trim() };
        }


        private Task<bool> SaveForecastsAsync()
        {
            return ExecuteSafe(async () =>
            {
                var json = JsonConvert.SerializeObject(ForecastManager.GetForecastCache());
                return await _storageService.SetCachedTextFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.WeatherCache).Description, json);
            });
        }

        public Task<bool> ActualizeAsync()
        {
            return ExecuteSafe(async () =>
            {
                foreach (var forecast in ForecastManager.GetForecasts())
                {
                    Uri url = GetApiUrl(forecast);
                    var service = new HttpService();
                    var feedresult = await service.DownloadAsync(url);
                    if (feedresult.IsRequestSuccessfull)
                        OpenWeatherMapHelper.EvaluateFeed(await feedresult.GetResponseAsStringAsync(),
                            _weatherFontMapping, forecast);
                }
                await SaveForecastsAsync();
                return true;
            });
        }
    }
}
