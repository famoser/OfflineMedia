using System;
using System.Collections.ObjectModel;
using OfflineMedia.Business.Models.Base;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Models
{
    public class FeedModel : BaseGuidModel
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Uri GetLogicUri()
        {
            return new Uri(Source.LogicBaseUrl + Url);
        }
        
        public ObservableCollection<ArticleModel> ArticleList { get; } = new ObservableCollection<ArticleModel>();

        public SourceModel Source { get; set; }
    }
}
