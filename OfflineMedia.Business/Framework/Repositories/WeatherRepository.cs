using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Models.WeatherModel;
using OfflineMedia.Business.Sources.OpenWeatherMap;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Services.Interfaces;

namespace OfflineMedia.Business.Framework.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private IStorageService _storageService;
        private Dictionary<string, string> _weatherFontMapping;
        private static string _apiUrl = "http://api.openweathermap.org/data/2.5/forecast?q={city}&lang=de";

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

                string feedresult = await Download.DownloadStringAsync(GetApiUrl(cityName));
                if (feedresult != null)
                {
                    var forecast = OpenWeatherMapHelper.Instance.EvaluateFeed(feedresult, _weatherFontMapping);
                    return forecast;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.Error, this, "GetForecastFor failed", ex);
            }
            return null;
        }
    }
}
