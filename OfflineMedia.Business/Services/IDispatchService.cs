using System;

namespace OfflineMedia.Business.Services
{
    public interface IDispatchService
    {
        // ReSharper disable once InconsistentNaming
        void CheckBeginInvokeOnUI(Action action);
    }
}
