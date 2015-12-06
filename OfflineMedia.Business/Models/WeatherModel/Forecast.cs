using System;
using System.Collections.Generic;
using System.Linq;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Models.WeatherModel
{
    public class Forecast : BaseModel
    {
        public string City { get; set; }

        public ForecastItem CurrentForecast
        {
            get
            {
                if (ForecastItems == null || !ForecastItems.Any())
                    return null;
                if (ActiveIndex - 1 > ForecastItems.Count)
                    ActiveIndex = 0;
                return ForecastItems[ActiveIndex];
            }
        }

        public ForecastItem Next1Forecast
        {
            get
            {
                if (ForecastItems == null || !ForecastItems.Any() || ActiveIndex > ForecastItems.Count)
                    return null;
                return ForecastItems[ActiveIndex + 1];
            }
        }

        public ForecastItem Next2Forecast
        {
            get
            {
                if (ForecastItems == null || !ForecastItems.Any() || ActiveIndex + 1 > ForecastItems.Count)
                    return null;
                return ForecastItems[ActiveIndex + 2];
            }
        }

        public ForecastItem Next3Forecast
        {
            get
            {
                if (ForecastItems == null || !ForecastItems.Any() || ActiveIndex + 2 > ForecastItems.Count)
                    return null;
                return ForecastItems[ActiveIndex + 3];
            }
        }

        private int _activeIndex;
        public int ActiveIndex
        {
            get { return _activeIndex; }
            set
            {
                if (Set(ref _activeIndex, value))
                    RaisePropertyChanged(() => CurrentForecast);
            }
        }

        public List<ForecastItem> ForecastItems { get; set; }

        public void SetCurrentForecast()
        {
            ActiveIndex = GetCurrentForecast();
        }

        private int GetCurrentForecast()
        {
            if (ForecastItems != null && ForecastItems.Any())
            {
                for (int i = 0; i < ForecastItems.Count; i++)
                {
                    if (ForecastItems[i].Date > DateTime.Now)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
    }
}
