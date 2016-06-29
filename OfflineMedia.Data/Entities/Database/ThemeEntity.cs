using Famoser.SqliteWrapper.Entities;

namespace OfflineMedia.Data.Entities.Database
{
    public class ThemeEntity : BaseEntity
    {
        public string DisplayName { get; set; }
        public string SaveName { get; set; }
    }
}
