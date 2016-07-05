using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.SqliteWrapper.Attributes;

namespace Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels
{
    public class ImageContentModel : BaseContentModel
    {
        [EntityMap]
        public string Url { get; set; }

        private LoadingState _loadingState;
        [EntityMap, EntityConversion(typeof(int), typeof(LoadingState))]
        public LoadingState LoadingState
        {
            get { return _loadingState; }
            set { Set(ref _loadingState, value); }
        }

        private byte[] _image;
        [EntityMap]
        public byte[] Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        [EntityMap]
        public int GalleryId { get; set; }
        [EntityMap]
        public int GalleryIndex { get; set; }
        [EntityMap]
        public int TextContentId { get; set; }

        private TextContentModel _text;
        public TextContentModel Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }
    }
}
