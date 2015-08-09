using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace OfflineMediaV3.Business.Entities
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
