using System.Collections.ObjectModel;
using OfflineMediaV3.Business.Models.Configuration;

namespace OfflineMediaV3.Business.Models.NewsModel
{
    public class FeedModel : BaseModel
    {
        private ObservableCollection<ArticleModel> _articlelist;
        public ObservableCollection<ArticleModel> ArticleList
        {
            get { return _articlelist; }
            set { Set(ref _articlelist, value); }
        }

        public void RefreshArticleList()
        {
            RaisePropertyChanged(() => ArticleList);
        }

        public string CustomTitle { get; set; }

        public string Titel
        {
            get
            {
                if (CustomTitle != null)
                    return CustomTitle;
                if (FeedConfiguration != null)
                    return FeedConfiguration.Name;
                return "no title";
            }
        }

        private FeedConfigurationModel _feedConfiguration;
        public FeedConfigurationModel FeedConfiguration
        {
            get { return _feedConfiguration; }
            set
            {
                if (Set(ref _feedConfiguration, value))
                {
                    RaisePropertyChanged(() => Titel);
                }
            }
        }

        public SourceModel Source { get; set; }
    }
}
