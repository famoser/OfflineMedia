using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.Business.Sources
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
