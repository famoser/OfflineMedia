using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;

namespace OfflineMedia.Services
{
    public class ApiService : IApiService
    {
#pragma warning disable 1998
        public async Task UploadStats(Dictionary<string, string> activeSources)
#pragma warning restore 1998
        {
            var tc = new TelemetryClient(); 
            tc.TrackEvent("Sources Active", activeSources);
        }
    }
}
