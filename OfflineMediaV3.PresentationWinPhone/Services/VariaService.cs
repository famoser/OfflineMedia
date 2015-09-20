using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using OfflineMediaV3.Common.Framework.Services.Interfaces;

namespace OfflineMediaV3.Services
{
    class VariaService : IVariaService
    {
        public async Task<bool> OpenInBrowser(Uri url)
        {
            return await Launcher.LaunchUriAsync(url);
        }
    }
}
