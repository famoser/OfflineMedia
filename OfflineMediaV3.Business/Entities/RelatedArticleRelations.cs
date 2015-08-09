using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Entities
{
    public class RelatedArticleRelations : EntityBase
    {
        public int Article1Id { get; set; }
        public int Article2Id { get; set; }
    }
}
