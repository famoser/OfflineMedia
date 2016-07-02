using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.SqliteWrapper.Attributes;

namespace Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels
{
    public class ImageContentModel : BaseContentModel
    {
        [EntityMap]
        public string Url { get; set; }
        [EntityMap, EntityConversion(typeof(int), typeof(LoadingState))]
        public LoadingState LoadingState { get; set; }
        
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
