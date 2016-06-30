using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Data.Helpers;

namespace OfflineMedia.Business.Newspapers
{
    public abstract class BaseMediaSourceHelper : IMediaSourceHelper
    {
        public abstract Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel);

        public abstract Task<bool> EvaluateArticle(ArticleModel articleModel);
    }
}
