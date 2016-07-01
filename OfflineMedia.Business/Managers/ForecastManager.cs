using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.WeatherModel;

namespace OfflineMedia.Business.Managers
{
    public class ForecastManager
    {
        private static readonly ObservableCollection<Forecast> Forecasts = new ObservableCollection<Forecast>();

        public static void AddForecast(Forecast forecast)
        {
            Forecasts.Add(forecast);
        }

        public static ObservableCollection<Forecast> GetForecasts()
        {
            return Forecasts;
        }

        public static WeatherCacheModel GetForecastCache()
        {
            return new WeatherCacheModel()
            {
                Forecasts = Forecasts
            };
        }
    }
}
