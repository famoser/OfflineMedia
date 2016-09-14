using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.UniversalWindows.Helpers;
using Famoser.OfflineMedia.Business.Services.Interfaces;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.WinUniversal.Platform.Mock
{
    public class PlatformCodeServiceMock : IPlatformCodeService
    {
        public async Task<byte[]> DownloadResizeImage(Uri url, double maxHeight, double maxWidth)
        {
            return new byte[0];
        }

        public void CheckBeginInvokeOnUi(Action action, Func<Task> after = null)
        {
            action.Invoke();
            after?.Invoke();
        }

        public async Task<bool> OpenInBrowser(Uri url)
        {
            return true;
        }

        public async Task<bool> Share(Uri articleUri, string title, string description)
        {
            return true;
        }

        public int DeviceWidth()
        {
            return (int)ResolutionHelper.WidthOfDevice;
        }

        public int DeviceHeight()
        {
            return (int)ResolutionHelper.HeightOfDevice;
        }

        public async Task<bool> DeleteDatabaseFile()
        {
            return true;
        }

        public void ExitApplication()
        {

        }
    }
}
