using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Business.Sources;
using OfflineMediaV3.Business.Sources.Blick;
using OfflineMediaV3.Business.Sources.Nzz;
using OfflineMediaV3.Business.Sources.Postillon;
using OfflineMediaV3.Business.Sources.Tamedia;
using OfflineMediaV3.Business.Sources.ZwanzigMin;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.Business.Helpers
{
    public class FeedHelper : SingletonBase<FeedHelper>
    {
        public async Task<List<ArticleModel>>  DownloadFeed(FeedModel feed)
        {
            IMediaSourceHelper mediaSourceHelper =ArticleHelper.Instance.GetMediaSource(feed.Source.SourceConfiguration.Source);

            if (mediaSourceHelper != null)
            {
                string feedresult = await Download.DownloadStringAsync(new Uri(feed.FeedConfiguration.Url));
                var newfeed = await mediaSourceHelper.EvaluateFeed(feedresult, feed.Source.SourceConfiguration, feed.FeedConfiguration);

                foreach (var article in newfeed)
                {
                    article.FeedConfiguration = feed.FeedConfiguration;
                }

                return newfeed;
            }
            return null;
        }
    }
}
