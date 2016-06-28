using Famoser.SqliteWrapper.Entities;

namespace OfflineMedia.Data.Entities.Database.Relations
{
    public class ThemeArticleRelations : EntityBase
    {
        public int ThemeId { get; set; }
        public int ArticleId { get; set; }
    }
}
