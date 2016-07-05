using System;
using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.Base;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace Famoser.OfflineMedia.Business.Models
{
    public class FeedModel : BaseInfoModel
    {
        public string Url { get; set; }

        public Uri GetLogicUri()
        {
            return new Uri(Source.LogicBaseUrl + Url);
        }

        public ObservableCollection<ArticleModel> AllArticles { get; } = new ObservableCollection<ArticleModel>();
        public ObservableCollection<ArticleModel> ActiveArticles { get; } = new ObservableCollection<ArticleModel>();

        public SourceModel Source { get; set; }
    }
}
