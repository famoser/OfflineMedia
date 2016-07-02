using Famoser.SqliteWrapper.Entities;

namespace Famoser.OfflineMedia.Data.Entities.Database.Contents
{
    public class ContentEntity : BaseEntity
    {
        public int ParentId { get; set; }
        public int Index { get; set; }

        public int ContentType { get; set; }
        public int ContentId { get; set; }
    }
}
