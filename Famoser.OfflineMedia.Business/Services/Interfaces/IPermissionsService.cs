using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.OfflineMedia.Business.Services.Interfaces
{
    public interface IPermissionsService
    {
        Task<bool> CanDownload();
        Task<bool> CanDownloadFeeds();
        Task<bool> CanDownloadArticles();
        Task<bool> CanDownloadImages();

        void OverrideDownloadPermission(bool value);
    }
}
