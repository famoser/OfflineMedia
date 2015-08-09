using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.NewsModel;
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
            List<ArticleModel> newfeed = new List<ArticleModel>();
            string feedresult = await Download.DownloadStringAsync(new Uri(feed.FeedConfiguration.Url));
            if (feed.Source.Configuration.Source == SourceEnum.Nzz)
            {
                var nzzh = new NzzHelper();
                newfeed = nzzh.EvaluateFeed(feedresult, feed.Source.Configuration);
            }
            else if (feed.Source.Configuration.Source == SourceEnum.ZwanzigMin)
            {
                var zwanzigMinH = new ZwanzigMinHelper();
                newfeed = zwanzigMinH.EvaluateFeed(feedresult, feed.Source.Configuration);
            }
            else if (feed.Source.Configuration.Source == SourceEnum.Blick || feed.Source.Configuration.Source == SourceEnum.BlickAmAbend)
            {
                var blickh = new BlickHelper();
                newfeed = blickh.EvaluateFeed(feedresult, feed.Source.Configuration);
            }
            else if (feed.Source.Configuration.Source == SourceEnum.BaslerZeitung
                || feed.Source.Configuration.Source == SourceEnum.BernerZeitung
                || feed.Source.Configuration.Source == SourceEnum.DerBund
                || feed.Source.Configuration.Source == SourceEnum.Tagesanzeiger)
            {
                var tamediah = new TamediaHelper();
                newfeed = tamediah.EvaluateFeed(feedresult, feed.Source.Configuration);
            }
            else if (feed.Source.Configuration.Source == SourceEnum.Postillon)
            {
                var postillonh = new PostillonHelper();
                newfeed = postillonh.EvaluateFeed(feedresult, feed.Source.Configuration);
            }

            foreach (var article in newfeed)
            {
                article.Feed = feed;
            }
            

            return newfeed;
        }
    }
}
