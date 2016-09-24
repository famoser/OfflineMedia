using System;
using Famoser.OfflineMedia.Business.Models.Progress;
using Famoser.OfflineMedia.Business.Services.Interfaces;
using GalaSoft.MvvmLight;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class ProgressViewModel : ViewModelBase
    {
        private readonly IProgressService _progressService;

        public ProgressViewModel(IProgressService progressService)
        {
            _progressService = progressService;
            _progressService.ActiveProgressChanged += ProgressServiceOnActiveProgressChanged;
        }

        private void ProgressServiceOnActiveProgressChanged(object sender, EventArgs eventArgs)
        {
            ActiveProgress = _progressService.GetActiveProgress();
        }

        private ProgressModel _activeProgress;
        public ProgressModel ActiveProgress
        {
            get { return _activeProgress; }
            set { Set(ref _activeProgress, value); }
        }
    }
}
