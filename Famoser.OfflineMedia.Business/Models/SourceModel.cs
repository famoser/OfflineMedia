using System.Collections.Generic;
using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.Base;
using Famoser.OfflineMedia.Data.Enums;

namespace Famoser.OfflineMedia.Business.Models
{
    public class SourceModel : BaseInfoModel
    {
        public string Abbreviation { get; set; }
        public Sources Source { get; set; }
        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public ObservableCollection<FeedModel> ActiveFeeds { get; } = new ObservableCollection<FeedModel>();
        public ObservableCollection<FeedModel> Feeds { get; } = new ObservableCollection<FeedModel>();
    }
}
