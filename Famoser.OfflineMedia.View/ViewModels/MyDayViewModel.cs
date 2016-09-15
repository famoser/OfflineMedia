using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.OfflineMedia.Business.Models.WeatherModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class MyDayViewModel : ViewModelBase
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IProgressService _progressService;

        public MyDayViewModel(IWeatherRepository weatherRepository, IProgressService progressService)
        {
            _weatherRepository = weatherRepository;
            _progressService = progressService;

            _refreshCommand = new LoadingRelayCommand(Refresh);

            Forecasts = _weatherRepository.GetForecasts();
            if (!IsInDesignMode)
                Refresh();
        }

        private ObservableCollection<Forecast> _forecasts;
        public ObservableCollection<Forecast> Forecasts
        {
            get { return _forecasts; }
            set { Set(ref _forecasts, value); }
        }

        #region refresh
        private readonly LoadingRelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;
        
        private async Task Refresh()
        {
            await _weatherRepository.ActualizeAsync();
        }

        #endregion
    }
}
