using Famoser.SqliteWrapper.Entities;

namespace OfflineMedia.Data.Entities.Database.Relations
{
    public class FeedArticleRelationEntity : BaseEntity
    {
        public int ArticleId { get; set; }
        public string FeedId { get; set; }
        public int Index { get; set; }
    }
}
