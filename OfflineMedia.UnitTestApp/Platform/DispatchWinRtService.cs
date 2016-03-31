using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using OfflineMedia.Business.Services;

namespace OfflineMedia.Platform
{
    public class DispatchWinRtService : IDispatchService
    {
        public void CheckBeginInvokeOnUI(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}
