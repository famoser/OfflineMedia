using System.Collections.ObjectModel;

namespace Famoser.OfflineMedia.Business.Models.WeatherModel
{
    public class WeatherCacheModel
    {
        public WeatherCacheModel()
        {
            Forecasts = new ObservableCollection<Forecast>();
        }

        public ObservableCollection<Forecast> Forecasts { get; set; }
    }
}
