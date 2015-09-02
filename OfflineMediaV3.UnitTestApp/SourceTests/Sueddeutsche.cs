using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMediaV3.Business.Helpers;

namespace OfflineMediaV3.UnitTestApp.SourceTests
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
