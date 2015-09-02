using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMediaV3.Business.Helpers;
using OfflineMediaV3.UnitTestApp.SourceTests.Helpers;

namespace OfflineMediaV3.UnitTestApp.SourceTests
{
    [TestClass]
    public class FeedTests
    {
        [TestMethod]
        public async Task AllFeedsOnline()
        {
            //prepare
            var configmodels = await SourceTestHelper.Instance.GetSourceConfigs();

            //act
            foreach (var sourceConfigurationModel in configmodels)
            {
                foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels)
                {
                    var str = await Download.DownloadStringAsync(new Uri(feedConfigurationModel.Url));
                    if (str == null)
                        Assert.Fail("Feed download failed for Feed " + feedConfigurationModel.Name + " with url " + feedConfigurationModel.Url + " for source " + sourceConfigurationModel.SourceNameShort);
                }
            }
        }
    }
}