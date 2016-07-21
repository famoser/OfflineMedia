using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Helpers;
using Famoser.OfflineMedia.Business.Models;
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
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            IocHelper.InitializeContainer();
        }

        [TestMethod]
        public void AllMediaSourceHelpers()
        {
            var values = Enum.GetValues(typeof(Sources));

            foreach (var value in values)
            {
                var enu = (Sources)value;
                Assert.IsNotNull(ArticleHelper.GetMediaSource(enu, SimpleIoc.Default.GetInstance<IThemeRepository>()), "media source not found for source " + enu);
            }
        }

        [TestMethod]
        public async Task TestFeedEvaluation()
        {
            var configmodels = await SourceTestHelper.Instance.GetSourceConfigModels();

            using (var logger = new Logger("get_feed_article"))
            {
                var sourceStack = new ConcurrentStack<SourceModel>(configmodels);
                var tasks = new List<Task>();
                for (int i = 0; i < 5; i++)
                {
                    tasks.Add(TestFeedEvaluationSourceTask(sourceStack, logger));
                }
                await Task.WhenAll(tasks);
                
                Assert.IsFalse(logger.HasEntryWithFaillure(), "Faillure occurred! Log files at " + logger.GetSavePath());
                Debug.Write("successfull! Log files at " + logger.GetSavePath());
            }
        }

        private async Task TestFeedEvaluationSourceTask(ConcurrentStack<SourceModel> sources, Logger logger)
        {
            SourceModel source;
            while (sources.TryPop(out source))
            {
                var sourceLogEntry = new LogEntry()
                {
                    Content = "Testing " + source.Name
                };

                var feeds = new ConcurrentStack<FeedModel>(source.AllFeeds);
                var tasks = new List<Task>();
                for (int i = 0; i < 5; i++)
                {
                    tasks.Add(TestFeedEvaluationFeedTask(feeds, source, sourceLogEntry));
                }

                await Task.WhenAll(tasks);

                logger.AddLog(sourceLogEntry);
            }
        }

        private async Task TestFeedEvaluationFeedTask(ConcurrentStack<FeedModel> feeds, SourceModel source, LogEntry sourceLogEntry, bool testArticles = false)
        {
            FeedModel feed;
            while (feeds.TryPop(out feed))
            {
                var feedLogEntry = new LogEntry()
                {
                    Content = "Testing " + feed.Name
                };

                var msh = ArticleHelper.GetMediaSource(source.Source, SimpleIoc.Default.GetInstance<IThemeRepository>());
                var newArticles = await msh.EvaluateFeed(feed);
                foreach (var articleModel in newArticles)
                {
                    var articleLogEntry = new LogEntry()
                    {
                        Content = "Testing " + articleModel.Title + " (" + articleModel.LogicUri + ")"
                    };
                    AssertHelper.TestFeedArticleProperties(articleModel, articleLogEntry);

                    if (articleModel.LoadingState != LoadingState.Loaded && !await msh.EvaluateArticle(articleModel))
                    {
                        articleLogEntry.LogEntries.Add(new LogEntry()
                        {
                            Content = "Evaluation failed!",
                            IsFaillure = true
                        });
                    }
                    else
                        AssertHelper.TestFullArticleProperties(articleModel, articleLogEntry);
                    feedLogEntry.LogEntries.Add(articleLogEntry);
                }

                sourceLogEntry.LogEntries.Add(feedLogEntry);
            }
        }
    }
}
