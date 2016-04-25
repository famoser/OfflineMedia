using System;
using OfflineMedia.Business.Enums;

namespace OfflineMedia.Business.Services
{
    public interface IProgressService
    {
        void ShowIndeterminateProgress(IndeterminateProgressKey key);
        void HideIndeterminateProgress(IndeterminateProgressKey key);

        void ShowDecentInformationMessage(string message, TimeSpan timespan);

        void InitializePercentageProgress(string message, int totalProgress);
        void IncrementProgress();
        void HidePercentageProgress();
    }
}
