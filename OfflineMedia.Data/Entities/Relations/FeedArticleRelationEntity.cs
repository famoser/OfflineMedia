namespace OfflineMedia.Data.Entities.Relations
{
    public class FeedArticleRelationEntity : EntityIdBase
    {
        public int ArticleId { get; set; }
        public string FeedGuid { get; set; }
    }
}
