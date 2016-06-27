namespace OfflineMedia.Data.Entities.Contents
{
    public class ContentEntity : EntityIdBase
    {
        public int ArticleId { get; set; }
        public int Index { get; set; }

        public int ContentType { get; set; }
        public int ContentId { get; set; }
    }
}
