using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Entities
{
    public class GalleryEntity : EntityBase
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Html { get; set; }
    }
}
