using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using OfflineMedia.Business.Enums;
using OfflineMedia.View.Enums;

namespace OfflineMedia.View.ViewModels.Global
{
    public class ProgressViewModel : ViewModelBase
    {
        public ProgressViewModel()
        {
            if (IsInDesignMode)
            {
                ProgressMessage = "Sample progress message...";
                ActiveProgressValue = 80;
                IsPercentageProgress = true;
                MaxProgressValue = 100;
            }

            _messages = new Dictionary<IndeterminateProgressKey, string>();
            _messages.Add(IndeterminateProgressKey.ReadingOutArticles, "Artikel werden ausgelesen");
            _messages.Add(IndeterminateProgressKey.SavingSettings, "Einstellungen werden gespeichert");
            _messages.Add(IndeterminateProgressKey.FeedSaveToDatabase, "Feeds werden verarbeitet");
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

        private int _activeProgressValue;
        public int ActiveProgressValue
        {
            get { return _activeProgressValue; }
            set { Set(ref _activeProgressValue, value); }
        }

        private int _maxProgressValue;
        public int MaxProgressValue
        {
            get { return _maxProgressValue; }
            set { Set(ref _maxProgressValue, value); }
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
                IsIndeterminateProgress = false;
            }
        }
        #endregion

        #region percentage Progress

        public void HidePercentageProgress()
        {
            IsPercentageProgress = false;
            ProgressMessage = "";
            ActiveProgressValue = MaxProgressValue;
        }

        public void IncrementProgress()
        {
            ActiveProgressValue++;
        }

        public void InitializePercentageProgress(string message, int totalProgress)
        {
            IsPercentageProgress = true;
            MaxProgressValue = totalProgress;
            ActiveProgressValue = 0;
            ProgressMessage = message;
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
