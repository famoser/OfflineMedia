using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Enums.Settings;
using OfflineMedia.Business.Framework.Repositories.Interfaces;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.WeatherModel;
using OfflineMedia.Common.Framework.Services.Interfaces;

namespace OfflineMedia.View.ViewModels
{
    public class MyDayViewModel : ViewModelBase
    {
        private IWeatherRepository _weatherRepository;
        private ISettingsRepository _settingsRepository;
        private IStorageService _storageService;

        public MyDayViewModel(IWeatherRepository weatherRepository, ISettingsRepository settingsRepository, IStorageService storageService)
        {
            _weatherRepository = weatherRepository;
            _settingsRepository = settingsRepository;
            _storageService = storageService;
            _addNewToDo = new RelayCommand(AddNewToDo, () => CanAddNewToDo);
            _removeToDo = new RelayCommand<string>(RemoveToDo);

            if (IsInDesignMode)
            {
                Forecast1 = new Forecast
                {
                    ForecastItems = new List<ForecastItem>(),
                    City = "Basel (CH)"
                };
                for (int i = 0; i < 3; i++)
                {
                    Forecast1.ForecastItems.Add(new ForecastItem()
                    {
                        Description = "ein bisschen wolkig",
                        ConditionId = 801,
                        ConditionFontIcon = ((char)int.Parse("EB48", System.Globalization.NumberStyles.HexNumber)).ToString(),
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

                _toDos = new ObservableCollection<string>()
                {
                    "Clean files",
                    "Do other stuff",
                    "Look up lady gaga long story short and all the other stuff",
                    "Another very long text to observe: Another very long text to observe: Another very long text to observe",
                    "lorem ipsum",
                };
            }
            else
                Initialize();

            Messenger.Default.Register<Messages>(this, EvaluateMessages);
        }

        private void EvaluateMessages(Messages obj)
        {
            if (obj == Messages.RefreshWeather)
                RefreshWeather();
        }

        private SettingModel _city1;
        private SettingModel _city2;
        private async void Initialize()
        {
            _city1 = await _settingsRepository.GetSettingByKey(SettingKeys.WeatherCity1);
            _city2 = await _settingsRepository.GetSettingByKey(SettingKeys.WeatherCity2);

            var todos = await _settingsRepository.GetSettingByKey(SettingKeys.ToDoList);
            ToDos = JsonConvert.DeserializeObject<ObservableCollection<string>>(todos.Value);

            RefreshWeather();
        }


        private async void RefreshWeather()
        {
            if (_city1 != null && _city1.Value != "")
                Forecast1 = await _weatherRepository.GetForecastFor(_city1.Value);
            if (Forecast1 == null)
            {
                var citycontent1 = await _settingsRepository.GetSettingByKey(SettingKeys.WeatherCity1Content);
                if (citycontent1 != null)
                    Forecast1 = JsonConvert.DeserializeObject<Forecast>(citycontent1.Value);
            }
            else
            {
                var json = JsonConvert.SerializeObject(Forecast1);
                await _settingsRepository.SaveSettingByKey(SettingKeys.WeatherCity1Content, json);
            }

            Forecast1?.SetCurrentForecast();
            
            if (_city2 != null && _city2.Value != "")
                Forecast2 = await _weatherRepository.GetForecastFor(_city2.Value);

            if (Forecast2 == null)
            {
                var citycontent2 = await _settingsRepository.GetSettingByKey(SettingKeys.WeatherCity2Content);
                if (citycontent2 != null)
                    Forecast2 = JsonConvert.DeserializeObject<Forecast>(citycontent2.Value);
            }
            else
            {
                var json = JsonConvert.SerializeObject(Forecast2);
                await _settingsRepository.SaveSettingByKey(SettingKeys.WeatherCity2Content, json);
            }
            
            Forecast2?.SetCurrentForecast();
        }

        private Forecast _forecast1;
        public Forecast Forecast1
        {
            get { return _forecast1; }
            set { Set(ref _forecast1, value); }
        }

        private Forecast _forecast2;
        public Forecast Forecast2
        {
            get { return _forecast2; }
            set { Set(ref _forecast2, value); }
        }
        
        private string _newToDo;
        public string NewToDo
        {
            get { return _newToDo; }
            set { if (Set(ref _newToDo, value))
                    _addNewToDo.RaiseCanExecuteChanged(); }
        }

        private ObservableCollection<string> _toDos; 
        public ObservableCollection<string> ToDos
        {
            get { return _toDos; }
            set { Set(ref _toDos, value); }
        }

        #region Add new ToDo
        private readonly RelayCommand _addNewToDo;
        public ICommand AddNewToDoCommand
        {
            get { return _addNewToDo; }
        }

        private bool CanAddNewToDo
        {
            get { return !string.IsNullOrEmpty(_newToDo); }
        }

        private void AddNewToDo()
        {
            _toDos.Add(NewToDo);
            NewToDo = "";
            ToDoChanged();
        }
        #endregion

        #region Remove new ToDo
        private readonly RelayCommand<string> _removeToDo;
        public ICommand RemoveToDoCommand
        {
            get { return _removeToDo; }
        }

        private void RemoveToDo(string item)
        {
            if (_toDos.Contains(item))
                _toDos.Remove(item);
            ToDoChanged();
        }
        #endregion

        private async void ToDoChanged()
        {
            RaisePropertyChanged(() => ToDos);
            var todos = JsonConvert.SerializeObject(ToDos);
            await _settingsRepository.SaveSettingByKey(SettingKeys.ToDoList, todos);
        }

        public string HalloWelt { get { return "hallo welt"; } }
    }
}
