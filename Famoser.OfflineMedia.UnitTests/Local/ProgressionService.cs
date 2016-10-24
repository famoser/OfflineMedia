using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums;
using Famoser.OfflineMedia.Business.Models.Progress;
using Famoser.OfflineMedia.Business.Services.Interfaces;

namespace Famoser.OfflineMedia.UnitTests.Local
{
    public class ProgressionService : IProgressService
    {
        public void StartIndeterminate(ProgressType type)
        {
            
        }

        public void Start(ProgressType type, int maxValue)
        {
            
        }

        public void IncreaseMaxValue(ProgressType type, int amount)
        {
            
        }

        public void Incremenent(ProgressType type)
        {
            
        }

        public void Stop(ProgressType type)
        {
            
        }

        public ProgressModel GetActiveProgress()
        {
            return null;
        }

        public event EventHandler ActiveProgressChanged;
    }
}
