using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Threading;
using OfflineMedia.Business.Services;

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
