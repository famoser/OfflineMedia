using System;
using SQLite.Net.Attributes;

namespace OfflineMediaV3.Data.Entities
{
    public class EntityBase
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public DateTime ChangeDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
