using System;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.Business.Services.Interfaces;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.UnitTests.Services.Mocks
{
    public class PlatformCodeServiceMock : IPlatformCodeService
    {
        public Task<byte[]> DownloadResizeImage(Uri url, double maxHeight, double maxWidth)
        {
            throw new NotImplementedException();
        }

        public void CheckBeginInvokeOnUi(Action action, Func<Task> after = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> OpenInBrowser(Uri url)
        {
            return true;
        }

        public int DeviceWidth()
        {
            throw new NotImplementedException();
        }

        public int DeviceHeight()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDatabaseFile()
        {
            throw new NotImplementedException();
        }

        public void ExitApplication()
        {
            throw new NotImplementedException();
        }
    }
}
