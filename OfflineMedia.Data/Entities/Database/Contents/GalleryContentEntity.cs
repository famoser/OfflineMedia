using Famoser.SqliteWrapper.Entities;

namespace Famoser.OfflineMedia.Data.Entities.Database.Contents
{
    public class GalleryContentEntity : BaseEntity
    {
        public string Url { get; set; }
        public int TextContentId { get; set; }
        public int LoadingState { get; set; }
    }
}
