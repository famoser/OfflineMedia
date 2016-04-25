using System;
using System.Threading.Tasks;

namespace OfflineMedia.Business.Services
{
    public interface IVariaService
    {
        Task<bool> OpenInBrowser(Uri url);
    }
}
