using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Sources.Welt
{
    public class WeltHelper : IMediaSourceHelper
    {
        public Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm)
        {
            throw new NotImplementedException();
        }

        public bool NeedsToEvaluateArticle()
        {
            throw new NotImplementedException();
        }

        public Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am)
        {
            throw new NotImplementedException();
        }

        public bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            throw new NotImplementedException();
        }

        public List<string> GetKeywords(ArticleModel articleModel)
        {
            throw new NotImplementedException();
        }
    }
}
