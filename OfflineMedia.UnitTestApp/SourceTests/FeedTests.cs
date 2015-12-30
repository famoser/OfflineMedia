using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Helpers;
using OfflineMedia.SourceTests.Helpers;

namespace OfflineMedia.SourceTests
{
    [TestClass]
    public class FeedTests
    {
        [TestMethod]
        public async Task AllFeedsOnline()
        {
            var invalidDic = new List<Tuple<string, string, string>>();

            //prepare
            var configmodels = await SourceTestHelper.Instance.GetSourceConfigs();

            //act
            foreach (var sourceConfigurationModel in configmodels)
            {
                foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels)
                {
                    var str = await Download.DownloadStringAsync(new Uri(feedConfigurationModel.Url));
                    if (str == null)
                        invalidDic.Add(new Tuple<string, string, string>(feedConfigurationModel.Name, feedConfigurationModel.Url, sourceConfigurationModel.SourceNameShort));
                }
            }
            if (invalidDic.Count > 0)
            {
                var msg = invalidDic.Aggregate("Feed download failed for Feeds: ", (current, tuple) => current + (tuple.Item1 + " with url " + tuple.Item2 + " for source " + tuple.Item3));
                Assert.Fail(msg);
            }
        }
    }
}