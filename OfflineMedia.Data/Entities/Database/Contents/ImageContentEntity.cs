using Famoser.SqliteWrapper.Entities;

namespace OfflineMedia.Data.Entities.Database.Contents
{
    public class ImageContentEntity : EntityBase
    {
        public string Url { get; set; }
        public int TextContentId { get; set; }
        public int LoadingState { get; set; }
        
        public byte[] Image { get; set; }
        
        public int GalleryId { get; set; }
        public int GalleryIndex { get; set; }
    }
}
