using System;
using OfflineMedia.Common.Enums.View;

namespace OfflineMedia.Common.Framework.Services.Interfaces
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
