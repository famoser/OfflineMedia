using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Entities
{
    public class ThemeArticleRelations : EntityBase
    {
        public int ThemeId { get; set; }
        public int ArticleId { get; set; }
    }
}
