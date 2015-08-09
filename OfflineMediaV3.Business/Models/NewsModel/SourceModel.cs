using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Models.Configuration;

namespace OfflineMediaV3.Business.Models.NewsModel
{
    public class SourceModel : BaseModel
    {
        public SourceConfigurationModel Configuration { get; set; }

        private ObservableCollection<FeedModel> _feedlist;
        public ObservableCollection<FeedModel> FeedList
        {
            get { return _feedlist; }
            set { Set(ref _feedlist, value); }
        }
    }
}
