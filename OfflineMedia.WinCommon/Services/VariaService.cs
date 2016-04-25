using System;
using System.Threading.Tasks;
using Windows.System;
using OfflineMedia.Business.Services;

namespace OfflineMedia.Services
{
    class VariaService : IVariaService
    {
        public async Task<bool> OpenInBrowser(Uri url)
        {
            return await Launcher.LaunchUriAsync(url);
        }
    }
}
