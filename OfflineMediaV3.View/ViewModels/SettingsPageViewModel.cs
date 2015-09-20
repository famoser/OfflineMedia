using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Settings;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Common.Enums.View;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMediaV3.View.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private List<SettingModel> _generalFreeSettings;
        public List<SettingModel> GeneralFreeSettings
        {
            get { return _generalFreeSettings; }
            set { Set(ref _generalFreeSettings, value); }
        }

        private List<SettingModel> _generalBoolSettings;
        public List<SettingModel> GeneralBoolSettings
        {
            get { return _generalBoolSettings; }
            set { Set(ref _generalBoolSettings, value); }
        }

        private List<SettingModel> _generalIntSettings;
        public List<SettingModel> GeneralIntSettings
        {
            get { return _generalIntSettings; }
            set { Set(ref _generalIntSettings, value); }
        }

        private List<SettingModel> _generalPossibleValuesSettings;
        public List<SettingModel> GeneralPossibleValuesSettings
        {
            get { return _generalPossibleValuesSettings; }
            set { Set(ref _generalPossibleValuesSettings, value); }
        }

        private List<SettingModel> _allSettings;

        private List<SourceConfigurationModel> _sourceConfiguration;
        public List<SourceConfigurationModel> SourceConfiguration
        {
            get { return _sourceConfiguration; }
            set { Set(ref _sourceConfiguration, value); }
        }

        private readonly ISettingsRepository _settingsRepository;
        private readonly IProgressService _progressService;
        private readonly INavigationService _navigationService;
        private readonly IStorageService _storageService;
        public SettingsPageViewModel(ISettingsRepository settingsRepository, IProgressService progressService, INavigationService navigationService, IStorageService storageService)
        {
            _settingsRepository = settingsRepository;
            _progressService = progressService;
            _navigationService = navigationService;
            _storageService = storageService;

            if (IsInDesignMode)
            {
                _allSettings = settingsRepository.GetSampleSettings();
                _sourceConfiguration = settingsRepository.GetSampleSourceConfiguration();
                _totalFileSize = 10000000;
                SortOutSettings();
            }
            else
                Initialize();

            _saveCommand = new RelayCommand(Save, () => CanSave);
            _clearSaveCommand = new RelayCommand(ClearSave, () => CanClearSave);
        }

        public async void Initialize()
        {
            using(var unitOfWork= new UnitOfWork(true))
            {
                _allSettings = await _settingsRepository.GetAllSettings(await unitOfWork.GetDataService());
                _sourceConfiguration = await _settingsRepository.GetSourceConfigurations(await unitOfWork.GetDataService());
            }

            SortOutSettings();

            TotalFileSize = await _storageService.GetFileSizes();
        }

        private void SortOutSettings()
        {
            GeneralFreeSettings = new List<SettingModel>();
            GeneralBoolSettings = new List<SettingModel>();
            GeneralIntSettings = new List<SettingModel>();
            GeneralPossibleValuesSettings = new List<SettingModel>();

            foreach (var item in _allSettings)
            {
                if (item.IsChangeable)
                {
                    if (item.IsCriticalChange)
                        item.PropertyChanged += CriticalPropertyChanged;
                    else
                        item.PropertyChanged += SomePropertyChanged;
                    if (item.ValueType == ValueTypeEnum.TrueOrFalse)
                        GeneralBoolSettings.Add(item);
                    else if (item.ValueType == ValueTypeEnum.Free)
                        GeneralFreeSettings.Add(item);
                    else if (item.ValueType == ValueTypeEnum.Int)
                        GeneralIntSettings.Add(item);
                    else if (item.ValueType == ValueTypeEnum.PossibleValues)
                        GeneralPossibleValuesSettings.Add(item);
                }
            }

            foreach (var sourceConfigurationModel in _sourceConfiguration)
            {
                sourceConfigurationModel.PropertyChanged += CriticalPropertyChanged;
                foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels)
                {
                    feedConfigurationModel.PropertyChanged += CriticalPropertyChanged;
                }
            }
        }

        private bool _propHasBeenChanged;
        private void SomePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propHasBeenChanged)
            {
                _propHasBeenChanged = true;
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _criticalChangeHasHappened;
        private void CriticalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propHasBeenChanged)
            {
                _criticalChangeHasHappened = true;
                _propHasBeenChanged = true;
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        #region save

        private RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        private bool CanSave
        {
            get { return _propHasBeenChanged && !_isSaving; }
        }

        private bool _isSaving;
        private async void Save()
        {
            _isSaving = true;
            _propHasBeenChanged = false;
            _saveCommand.RaiseCanExecuteChanged();

            _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.SavingSettings);

            await _settingsRepository.SaveSettings();

            _isSaving = false;
            _saveCommand.RaiseCanExecuteChanged();

            if (_criticalChangeHasHappened)
            {
                SimpleIoc.Default.Unregister(SimpleIoc.Default.GetInstance<MainPageViewModel>());
                Messenger.Default.Send(PageKeys.Main, Messages.ReloadGoBackPage);

                _criticalChangeHasHappened = false;
            }

            _progressService.HideIndeterminateProgress(IndeterminateProgressKey.SavingSettings);
        }

        #endregion

        #region clear save

        private bool _isClearingSave;
        private RelayCommand _clearSaveCommand;
        public ICommand ClearSaveCommand
        {
            get { return _clearSaveCommand; }
        }

        private bool CanClearSave
        {
            get { return !_isClearingSave; }
        }

        private async void ClearSave()
        {
            _isClearingSave = true;
            _progressService.ShowIndeterminateProgress(IndeterminateProgressKey.ClearSave);
            _clearSaveCommand.RaiseCanExecuteChanged();
            
            await _storageService.ClearFiles();

            _navigationService.GoBack();
            _navigationService.GoBack();
            _navigationService.GoBack();
            _navigationService.GoBack();
            
            _progressService.HideIndeterminateProgress(IndeterminateProgressKey.ClearSave);
        }

        #endregion
        
        private ulong _totalFileSize;
        public ulong TotalFileSize
        {
            get { return _totalFileSize; }
            set { Set(ref _totalFileSize, value); }
        }
    }
}
