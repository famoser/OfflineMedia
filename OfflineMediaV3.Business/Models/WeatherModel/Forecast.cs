using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.Business.Models.WeatherModel
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
    }
}
