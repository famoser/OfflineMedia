using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Models.WeatherModel
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
