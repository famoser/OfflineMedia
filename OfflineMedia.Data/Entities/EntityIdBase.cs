using SQLite.Net.Attributes;

namespace OfflineMedia.Data.Entities
{
    public class EntityIdBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}
