using System;
using OfflineMedia.Business.Services;

namespace OfflineMedia.Fakes
{
    class FakeDispatchService : IDispatchService
    {
        public void CheckBeginInvokeOnUI(Action action)
        {
            action.Invoke();
        }
    }
}
