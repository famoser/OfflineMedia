using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.Business.Entities
{
    public class ContentEntity : EntityBase
    {
        public int ContentType { get; set; }
        public int Order { get; set; }

        public string Html { get; set; }
        public int ImageId { get; set; }
        public int GalleryId { get; set; }

        public string ArticleUrl { get; set; }
    }
}
