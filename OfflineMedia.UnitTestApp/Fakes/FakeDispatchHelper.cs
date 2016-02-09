using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Services;

namespace OfflineMedia.Fakes
{
    class FakeDispatchHelper : IDispatchHelper
    {
        public void CheckBeginInvokeOnUI(Action action)
        {
            action.Invoke();
        }
    }
}
