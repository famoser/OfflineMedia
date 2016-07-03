using Famoser.SqliteWrapper.Entities;

namespace Famoser.OfflineMedia.Data.Entities.Database
{
    public class ThemeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}
