using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration.Base;
using OfflineMedia.Business.Repositories.Interfaces;
using IndeterminateProgressKey = OfflineMedia.Business.Enums.IndeterminateProgressKey;

namespace OfflineMedia.View.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly INavigationService _navigationService;
        private readonly IArticleRepository _articleRepository;
        private readonly IProgressService _progressService;

        public SettingsPageViewModel(ISettingsRepository settingsRepository,INavigationService navigationService, IArticleRepository articleRepository, IProgressService progressService)
        {
            _settingsRepository = settingsRepository;
            _navigationService = navigationService;
            _articleRepository = articleRepository;
            _progressService = progressService;

            if (IsInDesignMode)
            {
                Sources = _articleRepository.GetSampleSources();
                Settings = _settingsRepository.GetSampleSettings();
            }
            else
            {
                Sources = _articleRepository.GetAllSources();
                Settings = _settingsRepository.GetEditSettings();
            }

            _saveCommand = new RelayCommand(Save, () => CanSave);
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

        private RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;

        private bool CanSave => !_isSaving;

        private bool _isSaving;
        private async void Save()
        {
            _isSaving = true;
            _saveCommand.RaiseCanExecuteChanged();

            _progressService.StartIndeterminateProgress(IndeterminateProgressKey.SavingSettings);

            await _settingsRepository.SaveSettingsAsync();
            _progressService.StopIndeterminateProgress(IndeterminateProgressKey.SavingSettings);

            _isSaving = false;
            _saveCommand.RaiseCanExecuteChanged();
            
        }

        #endregion
    }
}
