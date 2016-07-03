using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.SqliteWrapper.Attributes;

namespace Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels
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
