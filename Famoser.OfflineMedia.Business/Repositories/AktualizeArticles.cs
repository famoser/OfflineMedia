using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums;
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

                if (!await _permissionsService.CanDownload())
                    return;

                var queue = new ConcurrentQueue<FeedModel>();
                foreach (var activeSource in SourceManager.GetActiveSources())
                    foreach (var activeFeed in activeSource.ActiveFeeds)
                        queue.Enqueue(activeFeed);

                var setting = await _settingsRepository.GetSettingByKeyAsync(SettingKey.ConcurrentThreads);
                var threadNumber = setting is IntSettingModel ? ((IntSettingModel)setting).IntValue : 5;

                var threads = new List<Task>();

                if (await _permissionsService.CanDownloadFeeds())
                {
                    _progressService.Start(ProgressType.Feed, queue.Count);
                    for (int i = 0; i < threadNumber; i++)
                    {
                        threads.Add(DoFeedQueue(queue));
                    }
                    await Task.WhenAll(threads.ToArray());
                    _progressService.Stop(ProgressType.Feed);
                    threads.Clear();
                }
                
                if (await _permissionsService.CanDownloadArticles())
                {
                    var stack = new ConcurrentStack<ArticleModel>();
                    foreach (var activeSource in SourceManager.GetActiveSources())
                        foreach (var activeFeed in activeSource.ActiveFeeds)
                        {
                            var news = activeFeed.AllArticles.Where(a => a.LoadingState < LoadingState.Loaded).ToArray();
                            if (news.Length > 0)
                                stack.PushRange(news);
                        }
                    //reverse stack so 
                    stack = new ConcurrentStack<ArticleModel>(stack.Reverse());

                    _progressService.Start(ProgressType.Article, stack.Count);
                    for (int i = 0; i < threadNumber; i++)
                    {
                        threads.Add(DoArticleStack(stack));
                    }
                    await Task.WhenAll(threads.ToArray());
                    _progressService.Stop(ProgressType.Article);
                    threads.Clear();
                }
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
                        if (await _permissionsService.CanDownloadFeeds())
                        {
                            var articles = await media.EvaluateFeed(model);
                            if (articles != null)
                                await SaveHelper.SaveFeed(model, articles, _sqliteService, _imageDownloadService);
                            _imageDownloadService.Download(model);
                        }
                    }

                    if (incrementProgress)
                        _progressService.Incremenent(ProgressType.Feed);
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
                    if (await _permissionsService.CanDownloadArticles())
                    {
                        if (media != null)
                        {
                            await media.EvaluateArticle(model);

                            model.LoadingState = LoadingState.Loaded;
                            await SaveHelper.SaveArticle(model, _sqliteService);
                            await SaveHelper.SaveArticleLeadImage(model, _sqliteService);
                            await SaveHelper.SaveArticleContent(model, _sqliteService);
                            _imageDownloadService.Download(model);
                        }
                        else
                        {
                            model.LoadingState = LoadingState.LoadingFailed;
                            await SaveHelper.SaveArticle(model, _sqliteService);
                        }
                    }

                    if (incrementProgress)
                        _progressService.Incremenent(ProgressType.Article);
                }
            });
        }

        public async Task ActualizeArticleAsync(ArticleModel am)
        {
            var stack = new ConcurrentStack<ArticleModel>();
            stack.Push(am);
            await DoArticleStack(stack);
        }
    }
}
