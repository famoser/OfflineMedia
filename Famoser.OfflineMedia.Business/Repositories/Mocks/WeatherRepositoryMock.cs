using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models.WeatherModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.Business.Repositories.Mocks
{
    public class WeatherRepositoryMock : IWeatherRepository
    {
        public ObservableCollection<Forecast> GetForecasts()
        {
            var forecasts = new ObservableCollection<Forecast>();
            for (int i = 0; i < 3; i++)
            {
                var forecast = new Forecast()
                {
                    City = "City " + i
                };

                for (int j = 0; j < 3; j++)
                {
                    forecast.Forecasts.Add(new ForecastItem()
                    {
                        Description = "ein bisschen wolkig",
                        ConditionId = 801,
                        ConditionFontIcon =
                            ((char)int.Parse("EB48", System.Globalization.NumberStyles.HexNumber)).ToString(),
                        CloudinessPercentage = 80,
                        HumidityPercentage = 16,
                        WindDegreee = 310,
                        WindSpeed = 12,
                        TemperatureKelvin = 287,
                        PressurehPa = 1300,
                        Date = DateTime.Now,
                        RainVolume = 0,
                        SnowVolume = 0
                    });
                }

                forecasts.Add(forecast);
            }

            return forecasts;
        }

        public async Task<bool> ActualizeAsync()
        {
            return true;
        }
    }
}
