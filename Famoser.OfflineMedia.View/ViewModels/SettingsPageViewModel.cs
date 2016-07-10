﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.View.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using IndeterminateProgressKey = Famoser.OfflineMedia.View.Enums.IndeterminateProgressKey;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IHistoryNavigationService _navigationService;
        private readonly IArticleRepository _articleRepository;
        private readonly IProgressService _progressService;

        public SettingsPageViewModel(ISettingsRepository settingsRepository, IHistoryNavigationService navigationService, IArticleRepository articleRepository, IProgressService progressService)
        {
            _settingsRepository = settingsRepository;
            _navigationService = navigationService;
            _articleRepository = articleRepository;
            _progressService = progressService;

            Sources = _articleRepository.GetAllSources();
            Settings = _settingsRepository.GetEditSettings();

            _saveCommand = new RelayCommand(Save, () => CanSave);
            _switchFeedStatusCommand = new RelayCommand<FeedModel>(SwitchFeedStatus, (fm) => CanSwitchFeedStatus);
            _switchSourceStatusCommand = new RelayCommand<SourceModel>(SwitchSourceStatus, (fm) => CanSwitchSourceStatus);
        }

        private ObservableCollection<BaseSettingModel> _settings;
        public ObservableCollection<BaseSettingModel> Settings
        {
            get { return _settings; }
            set { Set(ref _settings, value); }
        }

        private ObservableCollection<SourceModel> _sources;
        public ObservableCollection<SourceModel> Sources
        {
            get { return _sources; }
            set { Set(ref _sources, value); }
        }


        #region save
        private readonly RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;
        private bool CanSave => !IsSaving;
        public bool IsSaving { get; private set; }

        private async void Save()
        {
            using (new LoadingCommand(_saveCommand, (b) => IsSaving = b, IndeterminateProgressKey.SavingSettings, _progressService))
            {
                await _settingsRepository.SaveSettingsAsync();
            }
        }
        #endregion

        #region sourceStatusCommand
        private readonly RelayCommand<SourceModel> _switchSourceStatusCommand;
        public ICommand SwitchSourceStatusCommand => _switchSourceStatusCommand;
        private bool CanSwitchSourceStatus => !IsSwitchingSourceStatus;
        public bool IsSwitchingSourceStatus { get; private set; }

        private async void SwitchSourceStatus(SourceModel fm)
        {
            using (new LoadingCommandGeneric<SourceModel>(_switchSourceStatusCommand, (b) => IsSwitchingSourceStatus = b, IndeterminateProgressKey.SavingSourceSetting, _progressService))
            {
                await _articleRepository.SwitchSourceActiveStateAsync(fm);
            }
        }
        #endregion

        #region feedStatusCommand
        private readonly RelayCommand<FeedModel> _switchFeedStatusCommand;
        public ICommand SwitchFeedStatusCommand => _switchFeedStatusCommand;
        private bool CanSwitchFeedStatus => !IsSwitchingFeedStatus;
        public bool IsSwitchingFeedStatus { get; private set; }

        private async void SwitchFeedStatus(FeedModel fm)
        {
            using (new LoadingCommandGeneric<FeedModel>(_switchFeedStatusCommand, (b) => IsSwitchingFeedStatus = b, IndeterminateProgressKey.SavingFeedSetting, _progressService))
            {
                await _articleRepository.SwitchFeedActiveStateAsync(fm);
            }
        }
        #endregion
    }
}