using System;
using System.Threading.Tasks;
using Windows.System;
using OfflineMedia.Common.Framework.Services.Interfaces;

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
