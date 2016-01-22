using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources;
using OfflineMedia.Common.Framework.Logs;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.Business.Helpers
{
    public class FeedHelper : SingletonBase<FeedHelper>
    {
        private const int ConcurrentThreads = 5;
        public async Task<List<ArticleModel>> DownloadFeed(FeedModel feed)
        {
            IMediaSourceHelper mediaSourceHelper =ArticleHelper.Instance.GetMediaSource(feed.Source.SourceConfiguration.Source);

            if (mediaSourceHelper != null)
            {
                string feedresult = await Download.DownloadStringAsync(new Uri(feed.FeedConfiguration.Url));
                var newfeed = await mediaSourceHelper.EvaluateFeed(feedresult, feed.Source.SourceConfiguration, feed.FeedConfiguration);

                if (newfeed != null)
                {
                    foreach (var article in newfeed)
                    {
                        article.FeedConfiguration = feed.FeedConfiguration;
                    }

                    return newfeed;
                }
            }
            return null;
        }

        public async Task DownloadPictures(List<ArticleModel> models)
        {
            _toDo.AddRange(models);
            for (int i = 0; i < ConcurrentThreads; i++)
            {
                tsks.Add(DownloadImages());
            }

            while (tsks.Count > 0)
            {
                var tsk = tsks.FirstOrDefault();
                if (tsk.Status <= TaskStatus.Running)
                    await tsk;
                tsks.Remove(tsk);
            }
        }

        private List<Task> tsks = new List<Task>();
        private List<ArticleModel> _toDo = new List<ArticleModel>(); 
        private async Task DownloadImages()
        {
            try
            {
                while (_toDo.Count > 0)
                {
                    var art = _toDo.FirstOrDefault();
                    if (art != null)
                    {
                        _toDo.Remove(art);

                        if (art.LeadImage?.Url != null && !art.LeadImage.IsLoaded)
                        {
                            art.LeadImage.Image = await Download.DownloadImageAsync(art.LeadImage.Url);
                            art.LeadImage.IsLoaded = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, this);
            }
        }
    }
}
