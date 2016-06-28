namespace OfflineMedia.Data.Entities.Relations
{
    public class FeedArticleRelationEntity : EntityBase
    {
        public int ArticleId { get; set; }
        public string FeedId { get; set; }
        public int Index { get; set; }
    }
}
