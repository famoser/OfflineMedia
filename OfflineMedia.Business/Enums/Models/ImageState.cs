using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Enums.Models
{
    public enum LoadingState
    {
        New = 0,
        Loading = 1,
        Loaded = 2,

        LoadingFailed = 100
    }
}
