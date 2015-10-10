using System;
using System.Threading.Tasks;

namespace OfflineMedia.Common.Framework.Services.Interfaces
{
    public interface IVariaService
    {
        Task<bool> OpenInBrowser(Uri url);
    }
}
