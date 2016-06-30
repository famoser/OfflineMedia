using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Newspapers
{
    public interface IMediaSourceHelper
    {
        Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel);

        Task<bool> EvaluateArticle(ArticleModel articleModel);
    }
}
