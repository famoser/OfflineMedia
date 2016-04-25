using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Services
{
    public interface IApiService
    {
        Task UploadStats(Dictionary<string, string> activeSources);
    }
}
