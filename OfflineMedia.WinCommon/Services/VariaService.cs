using System;
using System.Threading.Tasks;
using Windows.System;

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
