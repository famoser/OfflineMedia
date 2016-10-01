using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.OfflineMedia.Business.Enums.Settings;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Configuration;
using Famoser.OfflineMedia.Business.Models.Configuration.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using Famoser.OfflineMedia.Data.Enums;
using GalaSoft.MvvmLight;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly IPlatformCodeService _platformCodeService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IPermissionsService _permissionsService;

        public SettingsPageViewModel(ISettingsRepository settingsRepository, IArticleRepository articleRepository, IPlatformCodeService platformCodeService, IPermissionsService permissionsService)
        {
            _settingsRepository = settingsRepository;
            _articleRepository = articleRepository;
            _platformCodeService = platformCodeService;
            _permissionsService = permissionsService;

            Sources = _articleRepository.GetAllSources();
            Settings = _settingsRepository.GetEditSettings();
            foreach (var baseSettingModel in Settings)
                baseSettingModel.PropertyChanged += BaseSettingModelOnPropertyChanged;


            _saveCommand = new LoadingRelayCommand(Save, () => _anythingChanged, true);
            _switchFeedStatusCommand = new LoadingRelayCommand<FeedModel>(SwitchFeedStatus, f => true, true);
            _switchSourceStatusCommand = new LoadingRelayCommand<SourceModel>(SwitchSourceStatus, f => true, true);
            _resetApplicationCommand = new LoadingRelayCommand(ResetApplication, () => true, true);

            if (!IsInDesignMode)
                InitializeSettingsAsync();
        }

        private void BaseSettingModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _anythingChanged = true;
            _saveCommand.RaiseCanExecuteChanged();
        }

        private IntSettingModel _concurrentThreadsSettingModel;
        private async void InitializeSettingsAsync()
        {
            var model = (IntSettingModel) await _settingsRepository.GetSettingByKeyAsync(SettingKey.ConcurrentThreads);
            ConcurrentThreadCount = model.Value;
            _concurrentThreadsSettingModel = model;
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

        public bool NormalDownloadImages
        {
            get { return _permissionsService.GetPermission(ConnectionType.Wlan, DownloadContentType.Image, true); }
            set { _permissionsService.SetPermission(ConnectionType.Wlan, DownloadContentType.Image, value); }
        }

        public bool MobileDownloadAny
        {
            get { return _permissionsService.GetPermission(ConnectionType.Mobile, DownloadContentType.Any, false); }
            set
            {
                _permissionsService.SetPermission(ConnectionType.Mobile, DownloadContentType.Any, value);
                _permissionsService.SetPermission(ConnectionType.Mobile, DownloadContentType.Article, value);
                _permissionsService.SetPermission(ConnectionType.Mobile, DownloadContentType.Feed, value);
            }
        }

        public bool MobileDownloadImages
        {
            get { return _permissionsService.GetPermission(ConnectionType.Mobile, DownloadContentType.Image, false); }
            set { _permissionsService.SetPermission(ConnectionType.Mobile, DownloadContentType.Image, value); }
        }

        private string _concurrentThreadCount = "5";
        public string ConcurrentThreadCount
        {
            get { return _concurrentThreadCount; }
            set
            {
                if (Set(ref _concurrentThreadCount, value))
                    TrySetConcurrentThread();
            }
        }

        private async void TrySetConcurrentThread()
        {
            int val;
            if (int.TryParse(ConcurrentThreadCount, out val))
            {
                //"firewall"
                if (val < 1 || val > 10)
                {
                    ConcurrentThreadCount = "5";
                    return;
                }
                if (_concurrentThreadsSettingModel != null)
                {
                    _concurrentThreadsSettingModel.IntValue = val;
                    await _settingsRepository.SaveSettingsAsync();
                }
            }
        }
    }
}
