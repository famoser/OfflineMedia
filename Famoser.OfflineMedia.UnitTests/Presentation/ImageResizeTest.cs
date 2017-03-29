using System;
using System.IO;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.OfflineMedia.WinUniversal.Platform;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.OfflineMedia.UnitTests.Presentation
{
    [TestClass]
    public class ImageResizeTest
    {
        [TestMethod]
        public async Task DownloadAndConvertImage()
        {
            //arrange
            var service = new HttpService();
            var imageResponse = await service.DownloadAsync(new Uri(TestImage));
            var image = await imageResponse.GetResponseAsByteArrayAsync();

            var ps = new PlatformCodeService();
            //act
            //resize
            var byteSmall = await ps.DownloadResizeImage(new Uri(TestImage), 200, 200);
            //do not resize
            var bytesOrigin = await ps.DownloadResizeImage(new Uri(TestImage), 10000, 10000);

            //assert
            Assert.IsTrue(image.Length == bytesOrigin.Length);

            //expected
            Assert.IsTrue(image.Length > byteSmall.Length);
        }

        private const string TestImage = "http://cdn3.spiegel.de/images/image-1123606-860_poster_16x9-lxpq-1123606.jpg";
    }
}
