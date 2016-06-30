using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Managers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Data.Entities.Database.Contents;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Repositories
{
    public partial class ArticleRepository
    {
        private bool _isActualizing;
        public Task ActualizeAllArticlesAsync()
        {
            return ExecuteSafe(async () =>
            {
                lock (this)
                {
                    if (_isActualizing)
                        return;

                    _isActualizing = true;
                }

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
                        stack.PushRange(activeFeed.ArticleList.ToArray());

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
                    var media = ArticleHelper.GetMediaSource(model);
                    if (media != null)
                    {
                        var articles = await media.EvaluateFeed(model);

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
                    var media = ArticleHelper.GetMediaSource(model);
                    if (media != null && await media.EvaluateArticle(model))
                    {
                        await _articleGenericRepository.SaveAsyc(model);
                        await ArticleHelper.SaveArticleLeadImage(model, _sqliteService);
                        await ArticleHelper.SaveArticleContent(model, _sqliteService);
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
