using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Famoser.OfflineMedia.Business.Models.Base;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace Famoser.OfflineMedia.Business.Models
{
    public class FeedModel : BaseInfoModel
    {
        public FeedModel()
        {
            AllArticles = new ObservableCollection<ArticleModel>();
            ActiveArticles = new ReadOnlyObservableCollection<ArticleModel>(AllArticles);
        }

        public string Url { get; set; }

        public Uri GetLogicUri()
        {
            return new Uri(Source.LogicBaseUrl + Url);
        }

        public ObservableCollection<ArticleModel> AllArticles { get; }
        public ReadOnlyObservableCollection<ArticleModel> ActiveArticles { get; }

        public SourceModel Source { get; set; }
    }
}
