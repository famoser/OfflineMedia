using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Newtonsoft.Json;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.WeatherModel;
using OfflineMedia.Business.Services;
using OfflineMedia.Business.Sources.OpenWeatherMap;

namespace OfflineMedia.Business.Framework.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private IStorageService _storageService;
        private Dictionary<string, string> _weatherFontMapping;
        private static string _apiUrl = "http://api.openweathermap.org/data/2.5/forecast?appid=3b2b694b8ac5add8b400dc24e563fd50&q={city}&lang=de";

        public WeatherRepository(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private Uri GetApiUrl(string cityName)
        {
            return new Uri(Uri.EscapeUriString(_apiUrl.Replace("{city}",  cityName)));
        }

        public async Task<Forecast> GetForecastFor(string cityName)
        {
            try
            {
                if (_weatherFontMapping == null)
                {
                    var json = await _storageService.GetWeatherFontJson();
                    _weatherFontMapping = json != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(json) : new Dictionary<string, string>();
                }

                Uri url = GetApiUrl(cityName);
                string feedresult = await Download.DownloadStringAsync(url);
                if (feedresult != null)
                {
                    var forecast = OpenWeatherMapHelper.EvaluateFeed(feedresult, _weatherFontMapping);
                    return forecast;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, "GetForecastFor failed",this, ex);
            }
            return null;
        }
    }
}
