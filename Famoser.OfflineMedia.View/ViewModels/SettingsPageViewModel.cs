using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Services;
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

            _saveCommand = new LoadingRelayCommand(Save);
            _switchFeedStatusCommand = new LoadingRelayCommand<FeedModel>(SwitchFeedStatus);
            _switchSourceStatusCommand = new LoadingRelayCommand<SourceModel>(SwitchSourceStatus);
            _resetApplicationCommand = new LoadingRelayCommand(ResetApplication);
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
        private readonly LoadingRelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private async void Save()
        {
            using (_saveCommand.GetProgressDisposable(_progressService, IndeterminateProgressKey.SavingSettings))
            {
                await _settingsRepository.SaveSettingsAsync();
            }
        }
        #endregion

        #region sourceStatusCommand
        private readonly LoadingRelayCommand<SourceModel> _switchSourceStatusCommand;
        public ICommand SwitchSourceStatusCommand => _switchSourceStatusCommand;

        private async void SwitchSourceStatus(SourceModel fm)
        {
            using (_switchSourceStatusCommand.GetProgressDisposable(_progressService, IndeterminateProgressKey.SavingSourceSetting))
            {
                await _articleRepository.SwitchSourceActiveStateAsync(fm);
            }
        }
        #endregion

        #region feedStatusCommand
        private readonly LoadingRelayCommand<FeedModel> _switchFeedStatusCommand;
        public ICommand SwitchFeedStatusCommand => _switchFeedStatusCommand;

        private async void SwitchFeedStatus(FeedModel fm)
        {
            using (_switchFeedStatusCommand.GetProgressDisposable(_progressService, IndeterminateProgressKey.SavingFeedSetting))
            {
                await _articleRepository.SwitchFeedActiveStateAsync(fm);
            }
        }
        #endregion

        #region reset application command
        private readonly LoadingRelayCommand _resetApplicationCommand;
        public ICommand ResetApplicationCommand => _resetApplicationCommand;

        private async void ResetApplication()
        {
            using (_resetApplicationCommand.GetProgressDisposable(_progressService, IndeterminateProgressKey.ResettingApplication))
            {
                await _settingsRepository.ResetApplicationAsync();
            }
            _platformCodeService.ExitApplication();
        }
        #endregion
    }
}
