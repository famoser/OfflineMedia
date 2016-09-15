using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Settings;

namespace Famoser.OfflineMedia.Business.Services.Interfaces
{
    public interface IPermissionsService
    {
        Task<bool> CanDownload();
        Task<bool> CanDownloadFeeds();
        Task<bool> CanDownloadArticles();
        Task<bool> CanDownloadImages();

        void SetPermission(ConnectionType conntype, DownloadContentType type, bool val);
        bool GetPermission(ConnectionType conntype, DownloadContentType type, bool fallback);

        void BlockDownloadPermission(bool value);
        
        event EventHandler PermissionsChanged;
    }
}
