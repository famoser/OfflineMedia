using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.OfflineMedia.Business.Models.WeatherModel;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Newspapers.OpenWeatherMap
{
    public class OpenWeatherMapHelper
    {
        public static void EvaluateFeed(string feed, Dictionary<string, string> weatherFontMapping, Forecast forecast)
        {
            forecast.Forecasts.Clear();
            var f = JsonConvert.DeserializeObject<Models.Forecast.RootObject>(feed);
            forecast.City = f.city.name;
            if (!string.IsNullOrEmpty(f.city.country))
                forecast.City += " (" + f.city.country + ")";
            foreach (var entry in f.list)
            {
                var item = new ForecastItem { Date = ConvertFromUnixTimestamp(entry.dt) };

                if (entry.main != null)
                {
                    item.HumidityPercentage = entry.main.humidity;
                    item.PressurehPa = entry.main.pressure;
                    item.TemperatureKelvin = entry.main.temp;
                }

                if (entry.weather != null && entry.weather.Any())
                {
                    var weather = entry.weather.FirstOrDefault();
                    item.ConditionId = weather.id;
                    if (weatherFontMapping.ContainsKey(weather.id.ToString()))
                        item.ConditionFontIcon = ((char)int.Parse(weatherFontMapping[weather.id.ToString()], System.Globalization.NumberStyles.HexNumber)).ToString();
                    item.Description = weather.description;
                }

                if (entry.clouds != null)
                {
                    item.CloudinessPercentage = entry.clouds.all;
                }

                if (entry.wind != null)
                {
                    item.WindDegreee = entry.wind.deg;
                    item.WindSpeed = entry.wind.speed;
                }

                if (entry.rain != null)
                {
                    item.RainVolume = entry.rain._3h;
                }

                if (entry.snow != null)
                {
                    item.RainVolume = entry.snow._3h;
                }

                forecast.Forecasts.Add(item);
            }
        }

        private static DateTime ConvertFromUnixTimestamp(int timestamp)
        {
            var original = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return original.AddSeconds(timestamp);

        }
    }
}
