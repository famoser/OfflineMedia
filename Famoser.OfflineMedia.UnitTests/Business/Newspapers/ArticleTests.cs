using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Helpers;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Enums;
using Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers;
using Famoser.OfflineMedia.UnitTests.Helpers;
using Famoser.OfflineMedia.UnitTests.Helpers.Models;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Logger = Famoser.OfflineMedia.UnitTests.Helpers.Logger;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers
{
    [TestClass]
    public class ArticleTests
    {
        [TestMethod]
        public void AllMediaSourceHelpers()
        {
            IocHelper.InitializeContainer();
            var values = Enum.GetValues(typeof(Sources));

            foreach (var value in values)
            {
                var enu = (Sources)value;
                Assert.IsNotNull(ArticleHelper.GetMediaSource(enu, SimpleIoc.Default.GetInstance<IThemeRepository>()), "media source not found for source " + enu);
            }
        }

        [TestMethod]
        public async Task GetFeedArticle()
        {
            var configmodels = await SourceTestHelper.Instance.GetSourceConfigs();
            IocHelper.InitializeContainer();
            var log = new List<string>();

            using (var logger = new Logger("get_feed_article"))
            {
                foreach (var sourceEntity in configmodels)
                {
                    var sourceModel = EntityModelConverter.Convert(sourceEntity);
                    var sourceLogEntry = new LogEntry()
                    {
                        Content = "Testing " + sourceModel.Name
                    };
                    var msh = ArticleHelper.GetMediaSource(sourceEntity.Source,
                        SimpleIoc.Default.GetInstance<IThemeRepository>());
                    foreach (var feedEntity in sourceEntity.Feeds)
                    {
                        var fm = EntityModelConverter.Convert(feedEntity, sourceModel, true);
                        var feedLogEntry = new LogEntry()
                        {
                            Content = "Testing " + feedEntity.Name
                        };

                        var newArticles = await msh.EvaluateFeed(fm);
                    }
                    logger.AddLog(sourceLogEntry);
                }
            }
        }

        [TestMethod]
        public async Task SpiegelGetFullArticle()
        {
            /*
            SourceTestHelper.Instance.PrepareTests();

            //arrange
            var sourceConfigs = await SourceTestHelper.Instance.GetSourceConfigs();
            var sourceConfig = sourceConfigs.FirstOrDefault(s => s.Source == SourceEnum.Spiegel);
            var feedConfig = sourceConfig.FeedConfigurationModels.FirstOrDefault();
            IMediaSourceHelper mediaSourceHelper = new SpiegelHelper();

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

                //author freiwillig
                articleModel.Author = " ";

                AssertHelper.Instance.AssertFeedArticleProperties(articleModel);
                AssertHelper.Instance.AssertFullArticleProperties(articleModel);
            }
            */
        }
    }
}
