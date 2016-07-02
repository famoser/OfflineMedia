using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Famoser.OfflineMedia.Data.Entities.Storage.Sources;
using Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers
{
    [TestClass]
    public class FeedTests
    {
        [TestMethod]
        public async Task AllFeedsOnline()
        {
            var invalids = new List<Tuple<FeedEntity, SourceEntity>>();;

            //prepare
            var configmodels = await SourceTestHelper.Instance.GetSourceConfigs();
            var service = new HttpService();

            //act
            foreach (var sourceConfigurationModel in configmodels)
            {
                foreach (var feedConfigurationModel in sourceConfigurationModel.Feeds)
                {
                    var resp = await service.DownloadAsync(new Uri(feedConfigurationModel.Url));
                    if (!resp.IsRequestSuccessfull)
                    {
                        invalids.Add(new Tuple<FeedEntity, SourceEntity>(feedConfigurationModel, sourceConfigurationModel));
                    }
                    else
                    {
                        var str = await resp.GetResponseAsStringAsync();
                        if (string.IsNullOrEmpty(str))
                            invalids.Add(new Tuple<FeedEntity, SourceEntity>(feedConfigurationModel, sourceConfigurationModel));
                    }
                }
            }
            if (invalids.Count > 0)
            {
                var response = invalids.Aggregate("", (current, invalid) => current + ("failed in " + invalid.Item2.Abbreviation + " with feed " + invalid.Item1.Name + "\n"));
                Assert.Fail(response);
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

                foreach (var feedConfigurationModel in sourceConfigurationModel.Feeds)
                {
                    if (guids.Contains(feedConfigurationModel.Guid))
                        invalidGuids.Add(feedConfigurationModel.Guid);
                    else
                        guids.Add(feedConfigurationModel.Guid);
                   }
            }
            if (invalidGuids.Count > 0)
            {
                var msg = invalidGuids.Aggregate("Feed guid duplicates: ", (current, guid) => current + guid + "\n");
                Assert.Fail(msg);
            }
        }
    }
}