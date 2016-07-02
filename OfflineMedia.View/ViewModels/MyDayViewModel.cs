using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Models.WeatherModel;
using OfflineMedia.Business.Repositories.Interfaces;

namespace OfflineMedia.View.ViewModels
{
    public class MyDayViewModel : ViewModelBase
    {
        private IWeatherRepository _weatherRepository;
        private readonly IProgressService _progressService;

        public MyDayViewModel(IWeatherRepository weatherRepository, IProgressService progressService)
        {
            _weatherRepository = weatherRepository;
            _progressService = progressService;

            _refreshCommand = new RelayCommand(Refresh, () => CanRefresh);

            if (IsInDesignMode)
            {
                Forecasts = _weatherRepository.GetSampleForecasts();
            }
            else
            {
                Forecasts = _weatherRepository.GetForecasts();
                Refresh();
            }
        }
        

        private ObservableCollection<Forecast> _forecasts;
        public ObservableCollection<Forecast> Forecasts
        {
            get { return _forecasts; }
            set { Set(ref _forecasts, value); }
        }

        #region refresh
        private RelayCommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand;

        private bool CanRefresh => !_isActualizing;

        private bool _isActualizing;
        private async void Refresh()
        {
            if (_isActualizing)
                return;

            _isActualizing = true;
            _refreshCommand.RaiseCanExecuteChanged();
            _progressService.StartIndeterminateProgress(IndeterminateProgressKey.RefreshingWeather);
            
            await _weatherRepository.ActualizeAsync();

            _progressService.StopIndeterminateProgress(IndeterminateProgressKey.RefreshingWeather);
            _isActualizing = false;
            _refreshCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}
