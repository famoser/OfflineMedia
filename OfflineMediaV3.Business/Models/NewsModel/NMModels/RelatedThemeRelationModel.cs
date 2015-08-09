using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Common.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel.NMModels
{
    public class RelatedThemeRelationModel
    {
        [EntityMap]
        public int Theme1Id { get; set; }

        [EntityMap]
        public int Theme2Id { get; set; }
    }
}
