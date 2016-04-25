using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.Configuration;
using OfflineMedia.Business.Services;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class ArticleModel : BaseModel
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

        [EntityMap]
        [EntityConversion(typeof(string), typeof(Uri))]
        public Uri PublicUri { get; set; }

        [EntityMap]
        [EntityConversion(typeof(string), typeof(Uri))]
        public Uri LogicUri { get; set; }


        private DateTime _publicationTime;
        [EntityMap]
        public DateTime PublicationTime
        {
            get { return _publicationTime; }
            set { Set(ref _publicationTime, value); }
        }

        private string _author;
        [EntityMap]
        public string Author
        {
            get { return _author; }
            set { Set(ref _author, value); }
        }

        private bool _isFavorite;
        [EntityMap]
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { Set(ref _isFavorite, value); }
        }

        private ArticleState _state;
        [EntityMap]
        [EntityConversion(typeof(int), typeof(ArticleState))]
        public ArticleState State
        {
            get { return _state; }
            set { Set(ref _state, value); }
        }

        [EntityMap]
        public int LeadImageId { get; set; }

        [EntityMap]
        [EntityConversion(typeof(string), typeof(Guid))]
        public Guid FeedConfigurationId { get; set; }

        [EntityMap]
        [EntityConversion(typeof(string), typeof(Guid))]
        public Guid SourceConfigurationId { get; set; }

        [EntityMap]
        public string WordDump { get; set; }

        private ImageModel _leadImage;
        public ImageModel LeadImage
        {
            get { return _leadImage; }
            set { Set(ref _leadImage, value); }
        }

        private List<ThemeModel> _themes;
        public List<ThemeModel> Themes
        {
            get { return _themes; }
            set { Set(ref _themes, value); }
        }

        private List<ArticleModel> _relatedArticles;
        public List<ArticleModel> RelatedArticles
        {
            get { return _relatedArticles; }
            set { Set(ref _relatedArticles, value); }
        }

        private List<ContentModel> _content;
        public List<ContentModel> Content
        {
            get { return _content; }
            set
            {
                SimpleIoc.Default.GetInstance<IDispatchService>().CheckBeginInvokeOnUI(() =>
                {
                    Set(ref _content, value);
                });
            }
        }

        public FeedConfigurationModel FeedConfiguration { get; set; }

        public ArticleModel LeftArticle { get; set; }

        public ArticleModel RightArticle { get; set; }

        public ArticleModel Link { get; set; }

        [CallBeforeSave]
        public void SetIds()
        {
            if (LeadImage != null)
                LeadImageId = LeadImage.Id;
            if (FeedConfiguration != null)
            {
                FeedConfigurationId = FeedConfiguration.Guid;
                SourceConfigurationId = FeedConfiguration.SourceConfiguration.Guid;
            }
        }

        public void PrepareForSave()
        {
            if (Content != null)
                foreach (var contentModel in Content)
                {
                    contentModel.Article = this;
                }
        }

        public bool IsLoadedCompletely()
        {
            return Content != null && Content.Any();
        }

        public bool IsInDatabase()
        {
            return Id != 0;
        }

        public bool IsStatic { get; set; }

        public static ArticleModel Clone(ArticleModel model)
        {
            return new ArticleModel()
            {
                Author = model.Author,
                Content = model.Content,
                FeedConfiguration = model.FeedConfiguration,
                FeedConfigurationId = model.FeedConfigurationId,
                IsFavorite = model.IsFavorite,
                IsStatic = model.IsStatic,
                LeadImage = model.LeadImage,
                LeadImageId = model.LeadImageId,
                LeftArticle = model.LeftArticle,
                LogicUri = model.LogicUri,
                PublicUri = model.PublicUri,
                PublicationTime = model.PublicationTime,
                RelatedArticles = model.RelatedArticles,
                RightArticle = model.RightArticle,
                SourceConfigurationId = model.SourceConfigurationId,
                State = model.State,
                SubTitle = model.SubTitle,
                Teaser = model.Teaser,
                Themes = model.Themes,
                Title = model.Title,
                WordDump = model.WordDump
            };
        }
    }
}
