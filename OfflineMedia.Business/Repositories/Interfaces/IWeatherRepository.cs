using System.Collections.ObjectModel;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.WeatherModel;

namespace OfflineMedia.Business.Repositories.Interfaces
{
    public interface IWeatherRepository
    {
        ObservableCollection<Forecast> GetForecasts();
        ObservableCollection<Forecast> GetSampleForecasts();

        Task<bool> ActualizeAsync();
    }
}
