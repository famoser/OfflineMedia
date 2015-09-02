using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace OfflineMediaV3.Data.Entities
{
    public class EntityIdBase
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
    }
}
