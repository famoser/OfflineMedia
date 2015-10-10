using System.Collections.ObjectModel;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.Business.Models
{
    public class SourceModel : BaseModel
    {
        public SourceConfigurationModel SourceConfiguration { get; set; }

        private ObservableCollection<FeedModel> _feedlist;
        public ObservableCollection<FeedModel> FeedList
        {
            get { return _feedlist; }
            set { Set(ref _feedlist, value); }
        }
    }
}
