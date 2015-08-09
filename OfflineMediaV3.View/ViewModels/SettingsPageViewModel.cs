using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.View;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Repositories;
using OfflineMediaV3.Business.Framework.Repositories.Interfaces;
using OfflineMediaV3.Business.Models.Configuration;

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
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private IArticleRepository _articleRepository;
        public SettingsPageViewModel(ISettingsRepository settingsRepository, IProgressService progressService, IDialogService dialogService, INavigationService navigationService, IArticleRepository articleRepository)
        {
            _settingsRepository = settingsRepository;
            _progressService = progressService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _articleRepository = articleRepository;

            if (IsInDesignMode)
            {
                _allSettings = settingsRepository.GetSampleSettings();
                _sourceConfiguration = settingsRepository.GetSampleSourceConfiguration();
                SortOutSettings();
            }
            else
                Initialize();

            _saveCommand = new RelayCommand(Save, () => CanSave);
        }

        public async void Initialize()
        {
            _allSettings = await _settingsRepository.GetAllSettings();
            _sourceConfiguration = await _settingsRepository.GetSourceConfigurations();
            SortOutSettings();
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
                foreach (var feedConfigurationModel in sourceConfigurationModel.Feeds)
                {
                    feedConfigurationModel.PropertyChanged += CriticalPropertyChanged;
                }
            }
        }

        private bool _propHasBeenChanged;
        private void SomePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!_propHasBeenChanged)
            {
                _propHasBeenChanged = true;
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _criticalChangeHasHappened;
        private void CriticalPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                SimpleIoc.Default.Unregister<MainPageViewModel>();
                SimpleIoc.Default.Register<MainPageViewModel>();
                Messenger.Default.Send(PageKeys.Main, Messages.ReloadGoBackPage);

                _criticalChangeHasHappened = false;
            }

            _progressService.HideIndeterminateProgress(IndeterminateProgressKey.SavingSettings);
        }

        #endregion
    }
}
