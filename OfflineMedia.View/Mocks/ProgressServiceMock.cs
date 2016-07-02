using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;

namespace Famoser.OfflineMedia.View.Mocks
{
    class ProgressServiceMock : IProgressService
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void ConfigurePercentageProgress(int maxValue, int activeValue = 0)
        {
            throw new NotImplementedException();
        }

        public void IncrementPercentageProgress()
        {
            throw new NotImplementedException();
        }

        public void SetPercentageProgress(int activeValue)
        {
            throw new NotImplementedException();
        }

        public void HidePercentageProgress()
        {
            throw new NotImplementedException();
        }

        public void StartIndeterminateProgress(object key)
        {
            throw new NotImplementedException();
        }

        public void StopIndeterminateProgress(object key)
        {
            throw new NotImplementedException();
        }

        public int GetPercentageProgressMaxValue()
        {
            throw new NotImplementedException();
        }

        public int GetPercentageProgressActiveValue()
        {
            throw new NotImplementedException();
        }

        public IList<object> GetActiveIndeterminateProgresses()
        {
            throw new NotImplementedException();
        }

        public bool IsPercentageProgressActive()
        {
            throw new NotImplementedException();
        }

        public bool IsIndeterminateProgressActive()
        {
            throw new NotImplementedException();
        }

        public bool IsAnyProgressActive()
        {
            throw new NotImplementedException();
        }
    }
}
