using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace Famoser.OfflineMedia.Business.Newspapers
{
    public interface IMediaSourceHelper
    {
        Task<List<ArticleModel>> EvaluateFeed(FeedModel feedModel);

        Task<bool> EvaluateArticle(ArticleModel articleModel);
    }
}
