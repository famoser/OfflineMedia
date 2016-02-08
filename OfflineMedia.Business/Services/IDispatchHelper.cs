using System;

namespace OfflineMedia.Business.Services
{
    public interface IDispatchHelper
    {
        // ReSharper disable once InconsistentNaming
        void CheckBeginInvokeOnUI(Action action);
    }
}
