using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Sources;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.Business.Helpers
{
    public class FeedHelper : SingletonBase<FeedHelper>
    {
        public async Task<List<ArticleModel>> DownloadFeed(FeedModel feed)
        {
            IMediaSourceHelper mediaSourceHelper =ArticleHelper.Instance.GetMediaSource(feed.Source.SourceConfiguration.Source);

            if (mediaSourceHelper != null)
            {
                string feedresult = await Download.DownloadStringAsync(new Uri(feed.FeedConfiguration.Url));
                var newfeed = await mediaSourceHelper.EvaluateFeed(feedresult, feed.Source.SourceConfiguration, feed.FeedConfiguration);

                foreach (var article in newfeed)
                {
                    article.FeedConfiguration = feed.FeedConfiguration;

                    if (article.LeadImage?.Url != null && !article.LeadImage.IsLoaded)
                    {
                        article.LeadImage.Image = await Download.DownloadImageAsync(article.LeadImage.Url);
                        article.LeadImage.IsLoaded = true;
                    }
                }

                return newfeed;
            }
            return null;
        }
    }
}
