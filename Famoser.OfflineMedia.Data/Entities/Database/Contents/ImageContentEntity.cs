using Famoser.SqliteWrapper.Entities;

namespace Famoser.OfflineMedia.Data.Entities.Database.Contents
{
    public class ImageContentEntity : BaseEntity
    {
        public string Url { get; set; }
        public int TextContentId { get; set; }
        public int LoadingState { get; set; }
        
        public byte[] Image { get; set; }
        
        public int GalleryId { get; set; }
        public int GalleryIndex { get; set; }
    }
}
