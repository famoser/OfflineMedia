using System;
using GalaSoft.MvvmLight.Threading;

namespace OfflineMedia.WinCommon.Services
{
    class DispatchWinRtService : IDispatchService
    {
        public void CheckBeginInvokeOnUI(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}
