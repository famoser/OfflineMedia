using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Famoser.OfflineMedia.WinUniversal.Services;
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
            var imageResponse = await service.DownloadAsync(new Uri("http://www.spiegel.de/images/image-1028227-hppano-lqbn.jpg"));
            var image = await imageResponse.GetResponseAsByteArrayAsync();

            var ps = new PlatformCodeService();
            //act
            //resize
            var byteSmall = await ps.DownloadResizeImage(new Uri("http://www.spiegel.de/images/image-1028227-hppano-lqbn.jpg"), 200, 200);
            //do not resize
            var bytesOrigin = await ps.DownloadResizeImage(new Uri("http://www.spiegel.de/images/image-1028227-hppano-lqbn.jpg"), 10000, 10000);

            //assert
            Assert.IsTrue(image.Length == bytesOrigin.Length);
            Assert.IsTrue(image.Length > byteSmall.Length);
        }
    }
}
