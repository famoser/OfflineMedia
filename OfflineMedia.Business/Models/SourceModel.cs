using System.Collections.ObjectModel;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Models
{
    public class SourceModel : BaseModel
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Data.Enums.Sources Source { get; set; }
        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public ObservableCollection<FeedModel> ActiveFeeds { get; } = new ObservableCollection<FeedModel>();
        public ObservableCollection<FeedModel> AllFeeds { get; } = new ObservableCollection<FeedModel>();
    }
}
