using System;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Services;

#pragma warning disable 1998
namespace Famoser.OfflineMedia.UnitTests.Services.Mocks
{
    public class PlatformCodeServiceMock : IPlatformCodeService
    {
        public async Task<byte[]> DownloadResizeImage(Uri url)
        {
            return new byte[] {12, 3, 41, 123};
        }

        public void CheckBeginInvokeOnUi(Action action)
        {
            action.Invoke();
        }

        public async Task<bool> OpenInBrowser(Uri url)
        {
            return true;
        }
    }
}
