using System.Collections.ObjectModel;
using System.Linq;
using Famoser.OfflineMedia.Business.Models.Base;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Models.WeatherModel
{
    public class Forecast : BaseModel
    {
        public string City { get; set; }

        public ObservableCollection<ForecastItem> Forecasts { get; } = new ObservableCollection<ForecastItem>();

        [JsonIgnore]
        public ForecastItem CurrentForecast
        {
            get
            {
                if (Forecasts == null || !Forecasts.Any() || ActiveIndex - 1 > Forecasts.Count)
                    return null;
                return Forecasts[ActiveIndex];
            }
        }

        private int _activeIndex;
        [JsonIgnore]
        public int ActiveIndex
        {
            get { return _activeIndex; }
            set
            {
                if (Set(ref _activeIndex, value))
                    RaisePropertyChanged(() => CurrentForecast);
            }
        }


        //public void SetCurrentForecast()
        //{
        //    ActiveIndex = GetCurrentForecast();
        //}

        //private int GetCurrentForecast()
        //{
        //    if (ForecastItems != null && ForecastItems.Any())
        //    {
        //        for (int i = 0; i < ForecastItems.Count; i++)
        //        {
        //            if (ForecastItems[i].Date > DateTime.Now)
        //            {
        //                return i;
        //            }
        //        }
        //    }
        //    return 0;
        //}
    }
}
