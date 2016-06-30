using System.Collections.ObjectModel;
using OfflineMedia.Business.Models.Base;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Business.Models
{
    public class SourceModel : BaseGuidModel
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Sources Source { get; set; }
        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public ObservableCollection<FeedModel> ActiveFeeds { get; } = new ObservableCollection<FeedModel>();
        public ObservableCollection<FeedModel> AllFeeds { get; } = new ObservableCollection<FeedModel>();
    }
}
