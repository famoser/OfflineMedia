using System;

namespace OfflineMedia.Data.Entities
{
    public class EntityBase : EntityIdBase
    {
        public DateTime ChangeDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
