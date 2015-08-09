using System.Collections.Generic;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Enums.Generic;
using OfflineMediaV3.Business.Enums.Models;
using OfflineMediaV3.Business.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel
{
    public class ContentModel : BaseModel
    {
        [EntityMap]
        [EntityConversion(typeof(int), typeof(ContentType))]
        public ContentType Type { get; set; }

        [EntityMap]
        public int Order { get; set; }

        [EntityMap]
        public string Html { get; set; }

        [EntityMap]
        public int ImageId { get; set; }
        public ImageModel Image { get; set; }

        [EntityMap]
        public int GalleryId { get; set; }
        public GalleryModel Gallery { get; set; }

        public List<ImageModel> Images { get; set; }
            
        [CallBeforeSave]
        public void SetIds()
        {
            if (Image != null)
                ImageId = Image.Id;

            if (Gallery != null)
                GalleryId = Gallery.Id;
        }
    }
}
