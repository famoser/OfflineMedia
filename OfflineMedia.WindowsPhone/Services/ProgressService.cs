using System;
using GalaSoft.MvvmLight.Ioc;
using OfflineMediaV3.Common.Enums.View;
using OfflineMediaV3.Common.Framework.Services.Interfaces;
using OfflineMediaV3.View.ViewModels.Global;

namespace OfflineMedia.WindowsPhone.Services
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

        public void ShowProgress(string message, int percentageCompleted)
        {
            _progressViewModel.ShowProgress(message, percentageCompleted);
        }

        public void HideProgress()
        {
            _progressViewModel.HideProgress();
        }
    }
}
