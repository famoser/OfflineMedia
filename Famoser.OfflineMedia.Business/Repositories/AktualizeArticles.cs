using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Helpers;
using Famoser.OfflineMedia.Business.Managers;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Configuration;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Data.Enums;

namespace Famoser.OfflineMedia.Business.Repositories
{
    public partial class ArticleRepository
    {
        public Task ActualizeAllArticlesAsync()
        {
            return ExecuteSafe(async () =>
            {
                await Initialize();

                var queue = new ConcurrentQueue<FeedModel>();
                foreach (var activeSource in SourceManager.GetActiveSources())
                    foreach (var activeFeed in activeSource.ActiveFeeds)
                        queue.Enqueue(activeFeed);

                var setting = await _settingsRepository.GetSettingByKeyAsync(SettingKey.ConcurrentThreads);
                var threadNumber = setting is IntSettingModel ? ((IntSettingModel)setting).IntValue : 5;

                _progressService.ConfigurePercentageProgress(queue.Count);
                var threads = new List<Task>();
                for (int i = 0; i < threadNumber; i++)
                {
                    threads.Add(DoFeedQueue(queue));
                }

                await Task.WhenAll(threads.ToArray());
                threads.Clear();

                var stack = new ConcurrentStack<ArticleModel>();
                foreach (var activeSource in SourceManager.GetActiveSources())
                    foreach (var activeFeed in activeSource.ActiveFeeds)
                    {
                        var news = activeFeed.AllArticles.Where(a => a.LoadingState < LoadingState.Loaded).ToArray();
                        if (news.Length > 0)
                            stack.PushRange(news);
                    }

                _progressService.ConfigurePercentageProgress(stack.Count);
                for (int i = 0; i < threadNumber; i++)
                {
                    threads.Add(DoArticleStack(stack));
                }

                await Task.WhenAll(threads.ToArray());
            });
        }

        private Task DoFeedQueue(ConcurrentQueue<FeedModel> queue, bool incrementProgress = true)
        {
            return ExecuteSafe(async () =>
            {
                FeedModel model;
                while (queue.TryDequeue(out model))
                {
                    var media = ArticleHelper.GetMediaSource(model, _themeRepository);
                    if (media != null)
                    {
                        var articles = await media.EvaluateFeed(model);
                        await FeedHelper.SaveFeed(model, articles, _sqliteService);
                        _imageDownloadService.Download(model);
                    }

                    if (incrementProgress)
                        _progressService.IncrementPercentageProgress();
                }
            });
        }

        private Task DoArticleStack(ConcurrentStack<ArticleModel> stack, bool incrementProgress = true)
        {
            return ExecuteSafe(async () =>
            {
                ArticleModel model;
                while (stack.TryPop(out model))
                {
                    model.LoadingState = LoadingState.Loading;
                    var media = ArticleHelper.GetMediaSource(model, _themeRepository);
                    if (media != null && await media.EvaluateArticle(model))
                    {
                        await ArticleHelper.SaveArticle(model, _sqliteService);
                        await ArticleHelper.SaveArticleLeadImage(model, _sqliteService);
                        await ArticleHelper.SaveArticleContent(model, _sqliteService);
                        model.LoadingState = LoadingState.Loaded;
                        _imageDownloadService.Download(model);
                    }
                    else
                    {
                        model.LoadingState = LoadingState.LoadingFailed;
                        await ArticleHelper.SaveArticle(model, _sqliteService);
                    }

                    if (incrementProgress)
                        _progressService.IncrementPercentageProgress();
                }
            });
        }

        public async Task ActualizeArticleAsync(ArticleModel am)
        {
            var stack = new ConcurrentStack<ArticleModel>();
            await DoArticleStack(stack);
        }
    }
}
