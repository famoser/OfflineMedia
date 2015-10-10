using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Sources;
using OfflineMedia.Business.Sources.Blick;
using OfflineMedia.UnitTestApp.SourceTests.Helpers;

namespace OfflineMedia.UnitTestApp.SourceTests
{
    [TestClass]
    public class Blick
    {
        [TestMethod]
        public async Task BlickGetFeedArticle()
        {
            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Blick);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new BlickHelper();

            //act
            var feed = await SourceTestHelper.Instance.GetFeedFor(mediaSourceHelper, sourceConfig, feedConfig);

            //assert
            Assert.IsTrue(feed.Any(), "Not items in feed");
            foreach (var articleModel in feed)
            {
                AssertHelper.Instance.AssertFeedArticleProperties(articleModel);
            }
        }

        [TestMethod]
        public async Task BlickGetFullArticle()
        {
            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Blick);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new BlickHelper();

            //act
            var feed = await SourceTestHelper.Instance.GetFeedFor(mediaSourceHelper, sourceConfig, feedConfig);

            //assert
            Assert.IsTrue(feed.Any(), "Not items in feed");
            for (int index = 0; index < feed.Count; index++)
            {
                var articleModel = feed[index];
                string articleString = await Download.DownloadStringAsync(articleModel.LogicUri);
                if (mediaSourceHelper.NeedsToEvaluateArticle())
                {
                    var tuple = await mediaSourceHelper.EvaluateArticle(articleString, articleModel);
                    if (tuple.Item1)
                    {
                        if (!mediaSourceHelper.WriteProperties(ref articleModel, tuple.Item2))
                            Assert.Fail("mediaSourceHelper WriteProperties failed for " + AssertHelper.Instance.GetArticleDescription(articleModel));
                    }
                    else
                        Assert.Fail("mediaSourceHelper EvaluateArticle failed for " + AssertHelper.Instance.GetArticleDescription(articleModel));
                }

                AssertHelper.Instance.AssertFeedArticleProperties(articleModel);
                AssertHelper.Instance.AssertFullArticleProperties(articleModel);
            }
        }
    }
}
