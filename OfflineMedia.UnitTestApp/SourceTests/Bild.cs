using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Enums;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Sources;
using OfflineMedia.Business.Sources.Bild;
using OfflineMedia.Business.Sources.Spiegel;
using OfflineMedia.SourceTests.Helpers;

namespace OfflineMedia.SourceTests
{
    [TestClass]
    public class Bild
    {
        [TestMethod]
        [Ignore]
        public async Task BildGetFeedArticle()
        {
            SourceTestHelper.Instance.PrepareTests();

            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Bild);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new BildHelper();

            //act
            var feed = await SourceTestHelper.Instance.GetFeedFor(mediaSourceHelper, sourceConfig, feedConfig);

            //assert
            Assert.IsTrue(feed.Any(), "No items in feed");
            foreach (var articleModel in feed)
            {
                //teaser is freiwillig
                AssertHelper.Instance.AssertFeedArticleProperties(articleModel);
            }
        }
        
        [TestMethod]
        [Ignore]
        public async Task BildGetFullArticle()
        {
            SourceTestHelper.Instance.PrepareTests();

            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Bild);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new BildHelper();

            //act
            var feed = await SourceTestHelper.Instance.GetFeedFor(mediaSourceHelper, sourceConfig, feedConfig);
            
            //assert
            Assert.IsTrue(feed.Any(), "No items in feed");
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