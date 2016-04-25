using System;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Services;
using OfflineMedia.View.ViewModels.Global;

namespace OfflineMedia.Services
{
    public class ProgressService : IProgressService
    {
        private ProgressViewModel _progressViewModel;

        public ProgressService()
        {
            _progressViewModel = SimpleIoc.Default.GetInstance<ProgressViewModel>();
        }

        public void ShowIndeterminateProgress(IndeterminateProgressKey key)
        {
            _progressViewModel.ShowIndeterminateProgress(key);
        }

        public void HideIndeterminateProgress(IndeterminateProgressKey key)
        {
            _progressViewModel.HideIndeterminateProgress(key);
        }

        public void ShowDecentInformationMessage(string message, TimeSpan timespan)
        {
            _progressViewModel.ShowDecentInformationMessage(message, timespan);
        }

        public void InitializePercentageProgress(string message, int totalProgress)
        {
            _progressViewModel.InitializePercentageProgress(message, totalProgress);
        }

        public void IncrementProgress()
        {
            _progressViewModel.IncrementProgress();
        }

        public void HidePercentageProgress()
        {
            _progressViewModel.HidePercentageProgress();
        }
    }
}
