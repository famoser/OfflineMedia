using Famoser.SqliteWrapper.Attributes;

namespace OfflineMedia.Business.Models.NewsModel.ContentModels
{
    public class ImageContentModel : BaseContentModel
    {
        [EntityMap]
        public string Url { get; set; }
        [EntityMap]
        public int LoadingState { get; set; }
        
        [EntityMap]
        public byte[] Image { get; set; }

        [EntityMap]
        public int GalleryId { get; set; }
        [EntityMap]
        public int GalleryIndex { get; set; }
        [EntityMap]
        public int TextContentId { get; set; }

        public TextContentModel Text { get; set; }
    }
}
