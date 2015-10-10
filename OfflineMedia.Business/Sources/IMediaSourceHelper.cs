using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Sources
{
    public interface IMediaSourceHelper
    {
        Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm);

        bool NeedsToEvaluateArticle();

        Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am);

        bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle);

        List<string> GetKeywords(ArticleModel articleModel);
    }
}
