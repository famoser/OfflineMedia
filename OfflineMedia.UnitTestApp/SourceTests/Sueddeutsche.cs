using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Helpers;

namespace OfflineMedia.SourceTests
{
    [TestClass]
    public class Sueddeutsche
    {
        [TestMethod]
        public async Task SueddeuscheAuthentication()
        {
            var res = await Download.DownloadStringAsync(new Uri("http://api.sueddeutsche.de/content?id=sz.1.2629856"));
            Assert.IsNotNull(res, "Authentication failed for Süddeutsche");
        }
    }
}
