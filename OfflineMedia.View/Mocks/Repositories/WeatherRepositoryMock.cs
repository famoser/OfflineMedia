using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.WeatherModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

namespace Famoser.OfflineMedia.View.Mocks.Repositories
{
    class WeatherRepositoryMock : IWeatherRepository
    {
        public ObservableCollection<Forecast> GetForecasts()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<Forecast> GetSampleForecasts()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ActualizeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
