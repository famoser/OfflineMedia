using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel
{
    public class ThemeModel : BaseModel
    {
        [EntityMap]
        public string Name { get; set; }
    }
}
