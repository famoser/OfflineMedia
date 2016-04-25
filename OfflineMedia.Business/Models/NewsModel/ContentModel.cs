using System.Collections.Generic;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class ContentModel : BaseModel
    {
        [EntityMap]
        public int ArticleId { get; set; }
        public ArticleModel Article { get; set; }

        [EntityMap]
        [EntityConversion(typeof(int), typeof(ContentType))]
        public ContentType ContentType { get; set; }

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
            if (Article != null)
                ArticleId = Article.Id;

            if (Image != null)
                ImageId = Image.Id;

            if (Gallery != null)
                GalleryId = Gallery.Id;
        }
    }
}
