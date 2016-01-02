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

        [TestMethod]
        public async Task ConfigurationValid()
        {
            //prepare
            var configmodels = await SourceTestHelper.Instance.GetSourceConfigs();
            var guids = new List<Guid>();
            var invalidGuids = new List<Guid>();

            //act
            foreach (var sourceConfigurationModel in configmodels)
            {
                if (guids.Contains(sourceConfigurationModel.Guid))
                    invalidGuids.Add(sourceConfigurationModel.Guid);
                else
                    guids.Add(sourceConfigurationModel.Guid);

                foreach (var feedConfigurationModel in sourceConfigurationModel.FeedConfigurationModels)
                {
                    if (guids.Contains(feedConfigurationModel.Guid))
                        invalidGuids.Add(feedConfigurationModel.Guid);
                    else
                        guids.Add(feedConfigurationModel.Guid);
                   }
            }
            if (invalidGuids.Count > 0)
            {
                var msg = invalidGuids.Aggregate("Guid duplicates: ", (current, guid) => current + guid + "; ");
                Assert.Fail(msg);
            }
        }
    }
}