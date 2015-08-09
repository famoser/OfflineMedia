using System.Collections.Generic;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Business.Models.Configuration;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.Business.Sources
{
    interface IMediaSourceHelper
    {
        List<ArticleModel> EvaluateFeed(string feed, SourceConfigurationModel scf);

        ArticleModel EvaluateArticle(string article, SourceConfigurationModel scf);
    }
}
