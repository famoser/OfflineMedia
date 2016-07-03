using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.WeatherModel;

namespace Famoser.OfflineMedia.Business.Managers
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
