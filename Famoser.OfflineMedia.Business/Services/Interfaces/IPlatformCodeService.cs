using System;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Settings;

namespace Famoser.OfflineMedia.Business.Services.Interfaces
{
    public interface IPlatformCodeService
    {
        Task<byte[]> DownloadResizeImage(Uri url, double maxHeight, double maxWidth);

        void CheckBeginInvokeOnUi(Action action, Func<Task> after = null);

        Task<bool> OpenInBrowser(Uri url);
        Task<bool> Share(Uri articleUri, string title, string description);

        int DeviceWidth();
        int DeviceHeight();

        Task<bool> DeleteDatabaseFile();
        void ExitApplication();

        ConnectionType GetConnectionType();

        object GetLocalSetting(string settingKey, object fallback);
        void SetLocalSetting(string settingKey, object value);
    }
}
