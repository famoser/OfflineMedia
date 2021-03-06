﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.OfflineMedia.Business.Models.WeatherModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using GalaSoft.MvvmLight;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class MyDayViewModel : ViewModelBase
    {
        private readonly IWeatherRepository _weatherRepository;

        public MyDayViewModel(IWeatherRepository weatherRepository, IProgressService progressService)
        {
            _weatherRepository = weatherRepository;

            _refreshCommand = new LoadingRelayCommand(Refresh);

            Forecasts = _weatherRepository.GetForecasts();
            if (!IsInDesignMode)
            {
                if (_refreshCommand.CanExecute(null))
                    _refreshCommand.Execute(null);
            }
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
