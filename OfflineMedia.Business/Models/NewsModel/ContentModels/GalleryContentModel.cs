using System.Collections.ObjectModel;
using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;

namespace OfflineMedia.Data.Entities.Contents
{
    public class GalleryContentModel : BaseContentModel
    {
        [EntityMap]
        public string Url { get; set; }

        private LoadingState _loadingState;
        [EntityMap]
        [EntityConversion(typeof(int), typeof(LoadingState))]
        public LoadingState LoadingState
        {
            get { return _loadingState; }
            set { Set(ref _loadingState, value); }
        }

        [EntityMap]
        public int TextContentId { get; set; }

        public ObservableCollection<ImageContentModel> Images { get; set; }
        public TextContentModel Text { get; set; }
    }
}
