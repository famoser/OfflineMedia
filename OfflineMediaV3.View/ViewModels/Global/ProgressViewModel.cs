using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OfflineMediaV3.Common.Enums.View;

namespace OfflineMediaV3.View.ViewModels.Global
{
    public class ProgressViewModel : ViewModelBase
    {
        public ProgressViewModel()
        {
            if (IsInDesignMode)
            {
                ProgressMessage = "Sample progress message...";
                ProgressPercentage = 80;
                IsPercentageProgress = true;
            }

            _messages = new Dictionary<IndeterminateProgressKey, string>();
            _messages.Add(IndeterminateProgressKey.ReadingOutArticles, "Artikel werden ausgelesen");
            _messages.Add(IndeterminateProgressKey.SavingSettings, "Einstellungen werden gespeichert");
        }

        public bool IsAnyProgressActive
        {
            get { return IsPercentageProgress || IsIndeterminateProgress || IsDecentInformation; }
        }

        private bool _isDecentInformation;
        public bool IsDecentInformation
        {
            get { return _isDecentInformation; }
            set
            {
                if (Set(ref _isDecentInformation, value))
                {
                    RaisePropertyChanged(() => IsAnyProgressActive);
                }
            }
        }

        private bool _isIndeterminateProgress;
        public bool IsIndeterminateProgress
        {
            get { return _isIndeterminateProgress; }
            set
            {
                if (Set(ref _isIndeterminateProgress, value))
                {
                    RaisePropertyChanged(() => IsAnyProgressActive);
                }
            }
        }

        private bool _isPercentageProgress;
        public bool IsPercentageProgress
        {
            get { return _isPercentageProgress; }
            set
            {
                if (Set(ref _isPercentageProgress, value))
                {
                    RaisePropertyChanged(() => IsAnyProgressActive);
                }
            }
        }

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get { return _progressPercentage; }
            set { Set(ref _progressPercentage, value); }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage ?? ""; }
            set { Set(ref _progressMessage, value); }
        }

        #region indeterminate Progress
        private Dictionary<IndeterminateProgressKey, string> _messages;
        public void ShowIndeterminateProgress(IndeterminateProgressKey key)
        {
            if (_messages.ContainsKey(key))
            {
                ProgressMessage = _messages[key];

                IsIndeterminateProgress = true;
            }
        }

        public void HideIndeterminateProgress(IndeterminateProgressKey key)
        {
            if (_messages.ContainsKey(key))
            {
                if (ProgressMessage == _messages[key])
                {
                    ProgressMessage = "";
                    IsIndeterminateProgress = false;
                }
            }
        }
        #endregion

        #region percentage Progress
        public void ShowProgress(string message, int percentageCompleted)
        {
            IsPercentageProgress = true;
            ProgressMessage = message;
            ProgressPercentage = percentageCompleted;
        }

        public void HideProgress()
        {
            IsPercentageProgress = false;
            ProgressMessage = "";
            ProgressPercentage = 100;
        }
        #endregion

        public async void ShowDecentInformationMessage(string message, TimeSpan timespan)
        {
            if (!IsPercentageProgress && !IsIndeterminateProgress)
            {
                IsDecentInformation = true;
                ProgressMessage = message;
            }

            await Task.Delay(timespan);

            if (ProgressMessage == message)
            {
                ProgressMessage = "";
                IsDecentInformation = false;
            }
        }
    }
}
