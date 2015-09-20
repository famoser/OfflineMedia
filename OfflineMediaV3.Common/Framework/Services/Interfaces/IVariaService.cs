using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Common.Framework.Services.Interfaces
{
    public interface IVariaService
    {
        Task<bool> OpenInBrowser(Uri url);
    }
}
