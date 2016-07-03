using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.WeatherModel;

namespace Famoser.OfflineMedia.Business.Repositories.Interfaces
{
    public interface IWeatherRepository
    {
        ObservableCollection<Forecast> GetForecasts();

        Task<bool> ActualizeAsync();
    }
}
