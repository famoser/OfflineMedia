using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.SqliteWrapper.Attributes;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Services;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class ArticleModel : BaseIdModel
    {
        public string Uri { get; set; }
        
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

        private ImageModel _leadImage;
        public ImageModel LeadImage
        {
            get { return _leadImage; }
            set { Set(ref _leadImage, value); }
        }
        
        public ObservableCollection<ThemeModel> Themes { get;  } = new ObservableCollection<ThemeModel>();
        
        public ObservableCollection<ArticleModel> RelatedArticles { get; } = new ObservableCollection<ArticleModel>();
        
        public ObservableCollection<ContentModel> Content { get; } = new ObservableCollection<ContentModel>();
    }
}
