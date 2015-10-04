using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.WeatherModel;

namespace OfflineMediaV3.Business.Framework.Repositories.Interfaces
{
    public interface IWeatherRepository
    {
        Task<Forecast> GetForecastFor(string cityName);
    }
}
