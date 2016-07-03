using System;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Models.Base;
using Newtonsoft.Json;

namespace Famoser.OfflineMedia.Business.Models.WeatherModel
{
    public class ForecastItem : BaseModel
    {
        public DateTime Date { get; set; }

        public string Description { get; set; }
        public int ConditionId { get; set; }
        public string ConditionFontIcon { get; set; }

        public double TemperatureKelvin { get; set; }
        public double PressurehPa { get; set; }

        public int HumidityPercentage { get; set; }
        public int CloudinessPercentage { get; set; }

        public double WindSpeed { get; set; }
        public double WindDegreee { get; set; }

        public double RainVolume { get; set; }
        public double SnowVolume { get; set; }

        [JsonIgnore]
        public WeatherLevel RainLevel
        {
            get
            {
                //source: http://wiki.sandaysoft.com/a/Rain_measurement
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (RainVolume == 0)
                    return WeatherLevel.None;
                if (RainVolume < 0.25)
                    return WeatherLevel.VeryLight;
                if (RainVolume < 1)
                    return WeatherLevel.Light;
                if (RainVolume < 4)
                    return WeatherLevel.Moderate;
                if (RainVolume < 16)
                    return WeatherLevel.Heavy;
                if (RainVolume < 50)
                    return WeatherLevel.VeryHeavy;
                return WeatherLevel.Extreme;
            }
        }

        [JsonIgnore]
        public WeatherLevel SnowLevel
        {
            get
            {
                //source: http://www.aqua-calc.com/calculate/volume-to-weight/substance/snow-coma-and-blank-freshly-blank-fallen
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (SnowVolume == 0)
                    return WeatherLevel.None;
                if (SnowVolume < 2.5)
                    return WeatherLevel.VeryLight;
                if (SnowVolume < 10)
                    return WeatherLevel.Light;
                if (SnowVolume < 40)
                    return WeatherLevel.Moderate;
                if (SnowVolume < 160)
                    return WeatherLevel.Heavy;
                if (SnowVolume < 500)
                    return WeatherLevel.VeryHeavy;
                return WeatherLevel.Extreme;
            }
        }

        [JsonIgnore]
        public WeatherLevel WindLevel
        {
            get
            {
                //source: http://www.aqua-calc.com/calculate/volume-to-weight/substance/snow-coma-and-blank-freshly-blank-fallen
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (WindSpeed == 0)
                    return WeatherLevel.None;
                if (WindSpeed < 5)
                    return WeatherLevel.VeryLight;
                if (WindSpeed < 19)
                    return WeatherLevel.Light;
                if (WindSpeed < 38)
                    return WeatherLevel.Moderate;
                if (WindSpeed < 74)
                    return WeatherLevel.Heavy;
                if (WindSpeed < 102)
                    return WeatherLevel.VeryHeavy;
                return WeatherLevel.Extreme;
            }
        }

        [JsonIgnore]
        public string WindDirection
        {
            get
            {
                var res = "S";
                if (WindDegreee < 90 || WindDegreee > 270)
                    res = "N";
                if (WindDegreee > 45 && WindDegreee < 135)
                    res += "O";
                else if (WindDegreee < 315 && WindDegreee > 225)
                    res += "W";
                return res;
            }
        }
    }
}
