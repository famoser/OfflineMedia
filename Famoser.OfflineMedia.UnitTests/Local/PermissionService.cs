using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Settings;
using Famoser.OfflineMedia.Business.Services.Interfaces;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.UnitTests.Local
{
    public class PermissionService : IPermissionsService
    {
        public async Task<bool> CanDownload()
        {
            return true;
        }

        public async Task<bool> CanDownloadFeeds()
        {
            return true;
        }

        public async Task<bool> CanDownloadArticles()
        {
            return true;
        }

        public async Task<bool> CanDownloadImages()
        {
            return true;
        }

        public void SetPermission(ConnectionType conntype, DownloadContentType type, bool val)
        {

        }

        public bool GetPermission(ConnectionType conntype, DownloadContentType type, bool fallback)
        {
            return true;
        }

        public void BlockDownloadPermission(bool value)
        {
        }

        public event EventHandler PermissionsChanged;
    }
}
