using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Helpers;
using OfflineMedia.Business.Managers;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
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

                foreach (var activeSource in SourceManager.GetActiveSources())
                    foreach (var activeFeed in activeSource.ActiveFeeds)
                        _feedQueue.Enqueue(activeFeed);

                var setting = await _settingsRepository.GetSettingByKeyAsync(SettingKey.ConcurrentThreads);
                var threadNumber = setting is IntSettingModel ? ((IntSettingModel)setting).IntValue : 5;

                _progressService.ConfigurePercentageProgress(_feedQueue.Count);
                var threads = new List<Task>();
                for (int i = 0; i < threadNumber; i++)
                {
                    threads.Add(DoFeedQueue());
                }

                await Task.WhenAll(threads.ToArray());
                threads.Clear();


                foreach (var activeSource in SourceManager.GetActiveSources())
                    foreach (var activeFeed in activeSource.ActiveFeeds)
                        _articleStack.PushRange(activeFeed.ArticleList.ToArray());

                for (int i = 0; i < threadNumber; i++)
                {
                    threads.Add(DoArticleStack());
                }

                await Task.WhenAll(threads.ToArray());
            });
        }
        
        private readonly ConcurrentQueue<FeedModel> _feedQueue = new ConcurrentQueue<FeedModel>();  
        private readonly ConcurrentStack<ArticleModel> _articleStack = new ConcurrentStack<ArticleModel>();

        private Task DoFeedQueue()
        {
            return ExecuteSafe(async () =>
            {
                FeedModel model;
                while (_feedQueue.TryDequeue(out model))
                {
                    var media = ArticleHelper.GetMediaSource(model);
                    if (media != null)
                        await media.EvaluateFeed(model);
                    _progressService.IncrementPercentageProgress();
                }
            });
        }

        private Task DoArticleStack(bool once = false)
        {
            return ExecuteSafe(async () =>
            {
                ArticleModel model;
                while (_articleStack.TryPop(out model))
                {
                    var media = ArticleHelper.GetMediaSource(model);
                    if (media != null)
                        await media.EvaluateArticle(model);

                    if (once)
                        return;

                    _progressService.IncrementPercentageProgress();
                }
            });
        }

        public async Task ActualizeArticleAsync(ArticleModel am)
        {
            _articleStack.Push(am);
            await DoArticleStack(true);
        }
    }
}
