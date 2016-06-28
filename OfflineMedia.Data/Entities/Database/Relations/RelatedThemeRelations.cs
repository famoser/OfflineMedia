using Famoser.SqliteWrapper.Entities;

namespace OfflineMedia.Data.Entities.Database.Relations
{
    public class RelatedThemeRelations : EntityBase
    {
        public int Theme1Id { get; set; }
        public int Theme2Id { get; set; }
    }
}
