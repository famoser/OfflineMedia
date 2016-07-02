using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Models.Base;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.SqliteWrapper.Attributes;

namespace Famoser.OfflineMedia.Business.Models.NewsModel
{
    public class ArticleModel : BaseIdModel
    {
        private string _title;
        [EntityMap]
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private string _subTitle;
        [EntityMap]
        public string SubTitle
        {
            get { return _subTitle; }
            set { Set(ref _subTitle, value); }
        }

        private string _teaser;
        [EntityMap]
        public string Teaser
        {
            get { return _teaser; }
            set { Set(ref _teaser, value); }
        }

        private string _author;
        [EntityMap]
        public string Author
        {
            get { return _author; }
            set { Set(ref _author, value); }
        }

        private bool _isRead;
        [EntityMap]
        public bool IsRead
        {
            get { return _isRead; }
            set { Set(ref _isRead, value); }
        }

        private bool _isFavorite;
        [EntityMap]
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { Set(ref _isFavorite, value); }
        }

        [EntityMap]
        public string PublicUri { get; set; }

        [EntityMap]
        public string LogicUri { get; set; }


        private LoadingState _loadingState;
        [EntityMap]
        [EntityConversion(typeof(int), typeof(LoadingState))]
        public LoadingState LoadingState
        {
            get { return _loadingState; }
            set { Set(ref _loadingState, value); }
        }

        private DateTime _publishDateTime;
        [EntityMap]
        public DateTime PublishDateTime
        {
            get { return _publishDateTime; }
            set { Set(ref _publishDateTime, value); }
        }

        private DateTime _downloadDateTime;
        [EntityMap]
        public DateTime DownloadDateTime
        {
            get { return _downloadDateTime; }
            set { Set(ref _downloadDateTime, value); }
        }

        private ImageContentModel _leadImage;
        public ImageContentModel LeadImage
        {
            get { return _leadImage; }
            set { Set(ref _leadImage, value); }
        }
        
        public ObservableCollection<ThemeModel> Themes { get;  } = new ObservableCollection<ThemeModel>();
        public ObservableCollection<ArticleModel> RelatedThemesArticles { get; } = new ObservableCollection<ArticleModel>();
        public ObservableCollection<ArticleModel> RelatedContentArticles { get; } = new ObservableCollection<ArticleModel>();
        public ObservableCollection<BaseContentModel> Content { get; } = new ObservableCollection<BaseContentModel>();

        public FeedModel Feed { get; set; }
        
        public Func<Task> AfterSaveFunc { get; set; }

        private string _wordDump;
        [EntityMap]
        public string WordDump
        {
            get { return _wordDump; }
            set { Set(ref _wordDump, value); }
        }
    }
}
