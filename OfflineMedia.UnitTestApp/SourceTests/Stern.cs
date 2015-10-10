using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Sources;
using OfflineMedia.Business.Sources.Stern;
using OfflineMedia.SourceTests.Helpers;

namespace OfflineMedia.SourceTests
{
    [TestClass]
    public class Stern
    {
        [TestMethod]
        public async Task SternGetFeedArticle()
        {
            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Stern);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new SternHelper();

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
        public async Task SternGetFullArticle()
        {
            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Stern);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new SternHelper();

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

                articleModel.Author += "-";
                AssertHelper.Instance.AssertFullArticleProperties(articleModel);
            }
        }
    }
}