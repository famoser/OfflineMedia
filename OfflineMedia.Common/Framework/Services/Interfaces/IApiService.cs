using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Common.Framework.Services.Interfaces
{
    public interface IApiService
    {
        Task UploadStats(Dictionary<string, string> activeSources);
    }
}
