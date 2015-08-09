using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Entities
{
    public class RelatedThemeRelations : EntityBase
    {
        public int Theme1Id { get; set; }
        public int Theme2Id { get; set; }
    }
}
