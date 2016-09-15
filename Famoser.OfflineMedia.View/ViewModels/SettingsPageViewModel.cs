using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight;
using IndeterminateProgressKey = Famoser.OfflineMedia.View.Enums.IndeterminateProgressKey;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IPlatformCodeService _platformCodeService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IProgressService _progressService;

        public SettingsPageViewModel(ISettingsRepository settingsRepository, IArticleRepository articleRepository, IProgressService progressService, IPlatformCodeService platformCodeService)
        {
            _settingsRepository = settingsRepository;
            _articleRepository = articleRepository;
            _progressService = progressService;
            _platformCodeService = platformCodeService;

            Sources = _articleRepository.GetAllSources();
            Settings = _settingsRepository.GetEditSettings();
            foreach (var baseSettingModel in Settings)
                baseSettingModel.PropertyChanged += BaseSettingModelOnPropertyChanged;


            _saveCommand = new LoadingRelayCommand(Save, () => _anythingChanged, true);
            _switchFeedStatusCommand = new LoadingRelayCommand<FeedModel>(SwitchFeedStatus, f => true, true);
            _switchSourceStatusCommand = new LoadingRelayCommand<SourceModel>(SwitchSourceStatus, f => true, true);
            _resetApplicationCommand = new LoadingRelayCommand(ResetApplication, () => true, true);
        }

        private void BaseSettingModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _anythingChanged = true;
            _saveCommand.RaiseCanExecuteChanged();
        }

        private bool _anythingChanged;

        public ObservableCollection<BaseSettingModel> Settings { get; }

        public ObservableCollection<SourceModel> Sources { get; }

        #region save
        private readonly LoadingRelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private async Task Save()
        {
            await _settingsRepository.SaveSettingsAsync();
            _anythingChanged = false;
            _saveCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region sourceStatusCommand
        private readonly LoadingRelayCommand<SourceModel> _switchSourceStatusCommand;
        public ICommand SwitchSourceStatusCommand => _switchSourceStatusCommand;

        private async Task SwitchSourceStatus(SourceModel fm)
        {
            await _articleRepository.SwitchSourceActiveStateAsync(fm);
        }
        #endregion

        #region feedStatusCommand
        private readonly LoadingRelayCommand<FeedModel> _switchFeedStatusCommand;
        public ICommand SwitchFeedStatusCommand => _switchFeedStatusCommand;

        private async Task SwitchFeedStatus(FeedModel fm)
        {
            await _articleRepository.SwitchFeedActiveStateAsync(fm);
        }
        #endregion

        #region reset application command
        private readonly LoadingRelayCommand _resetApplicationCommand;
        public ICommand ResetApplicationCommand => _resetApplicationCommand;

        private async Task ResetApplication()
        {
            await _settingsRepository.ResetApplicationAsync();
            _platformCodeService.ExitApplication();
        }
        #endregion
    }
}
