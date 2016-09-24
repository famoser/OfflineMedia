using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums;
using Famoser.OfflineMedia.Business.Models.Progress;

namespace Famoser.OfflineMedia.Business.Services.Interfaces
{
    public interface IProgressService
    {
        void StartIndeterminate(ProgressType type);
        void Start(ProgressType type, int maxValue);
        void IncreaseMaxValue(ProgressType type, int amount);
        void Incremenent(ProgressType type);
        void Stop(ProgressType type);

        ProgressModel GetActiveProgress();

        event EventHandler ActiveProgressChanged;
    }
}
