using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Common.Helpers;

namespace OfflineMedia.Business.Sources
{
    public abstract class BaseMediaSourceHelper : IMediaSourceHelper
    {
        public abstract Task<List<ArticleModel>> EvaluateFeed(string feed, SourceConfigurationModel scf, FeedConfigurationModel fcm);

        public virtual bool NeedsToEvaluateArticle()
        {
            return true;
        }

        public abstract Task<Tuple<bool, ArticleModel>> EvaluateArticle(string article, ArticleModel am);

        public virtual bool WriteProperties(ref ArticleModel original, ArticleModel evaluatedArticle)
        {
            original.Author = evaluatedArticle.Author;
            original.Content = evaluatedArticle.Content;
            return true;
        }

        public virtual List<string> GetKeywords(ArticleModel articleModel)
        {
            var part1 = TextHelper.Instance.GetImportantWords(articleModel.Title);
            var part2 = TextHelper.Instance.GetImportantWords(articleModel.SubTitle);

            return TextHelper.Instance.FusionLists(part1, part2);
        }
    }
}
