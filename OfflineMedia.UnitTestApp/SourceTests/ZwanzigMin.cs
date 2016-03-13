using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Sources;
using OfflineMedia.Business.Sources.ZwanzigMin;
using OfflineMedia.SourceTests.Helpers;

namespace OfflineMedia.SourceTests
{
    [TestClass]
    public class ZwanzigMin
    {
        [TestMethod]
        public async Task ZwanzigMinGetFeedArticle()
        {
            SourceTestHelper.Instance.PrepareTests();

            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.ZwanzigMin);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new ZwanzigMinHelper();

            //act
            var feed = await SourceTestHelper.Instance.GetFeedFor(mediaSourceHelper, sourceConfig, feedConfig);

            //assert
            Assert.IsTrue(feed.Any(), "Not items in feed");
            foreach (var articleModel in feed)
            {
                articleModel.LogicUri = new Uri("http://baslerzeitung.ch");
                AssertHelper.Instance.AssertFeedArticleProperties(articleModel);
            }
        }

        [TestMethod]
        public async Task ZwanzigMinGetFullArticle()
        {
            SourceTestHelper.Instance.PrepareTests();

            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.ZwanzigMin);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new ZwanzigMinHelper();

            //act
            var feed = await SourceTestHelper.Instance.GetFeedFor(mediaSourceHelper, sourceConfig, feedConfig);

            //assert
            Assert.IsTrue(feed.Any(), "Not items in feed");
            for (int index = 0; index < feed.Count; index++)
            {
                var articleModel = feed[index];
                if (mediaSourceHelper.NeedsToEvaluateArticle())
                {
                    string articleString = await Download.DownloadStringAsync(articleModel.LogicUri);
                    var tuple = await mediaSourceHelper.EvaluateArticle(articleString, articleModel);
                    if (tuple.Item1)
                    {
                        if (!mediaSourceHelper.WriteProperties(ref articleModel, tuple.Item2))
                            Assert.Fail("mediaSourceHelper WriteProperties failed for " + AssertHelper.Instance.GetArticleDescription(articleModel));
                    }
                    else
                        Assert.Fail("mediaSourceHelper EvaluateArticle failed for " + AssertHelper.Instance.GetArticleDescription(articleModel));
                }

                articleModel.LogicUri = new Uri("http://baslerzeitung.ch");
                AssertHelper.Instance.AssertFeedArticleProperties(articleModel);
                AssertHelper.Instance.AssertFullArticleProperties(articleModel);
            }
        }
    }
}
