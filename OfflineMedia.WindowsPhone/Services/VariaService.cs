using System;
using System.Threading.Tasks;
using Windows.System;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMedia.WindowsPhone.Services
{
    class VariaService : IVariaService
    {
        public async Task<bool> OpenInBrowser(Uri url)
        {
            return await Launcher.LaunchUriAsync(url);
        }
    }
}
