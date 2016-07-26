using System;
using System.Threading.Tasks;

namespace Famoser.OfflineMedia.Business.Services
{
    public interface IPlatformCodeService
    {
        Task<byte[]> DownloadResizeImage(Uri url, double maxHeight, double maxWidth);
        void CheckBeginInvokeOnUi(Action action, Func<Task> after = null);
        Task<bool> OpenInBrowser(Uri url);
        int DeviceWidth();
        int DeviceHeight();
        Task<bool> DeleteDatabaseFile();
        void ExitApplication();
    }
}
