using System;
using OfflineMediaV3.Common.Enums.View;

namespace OfflineMediaV3.Common.Framework.Services.Interfaces
{
    public interface IProgressService
    {
        void ShowIndeterminateProgress(IndeterminateProgressKey key);
        void HideIndeterminateProgress(IndeterminateProgressKey key);

        void ShowDecentInformationMessage(string message, TimeSpan timespan);

        void ShowProgress(string message, int percentageCompleted);
        void HideProgress();
    }
}
