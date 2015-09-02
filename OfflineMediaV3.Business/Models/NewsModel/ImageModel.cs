using System;
using OfflineMediaV3.Common.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel
{
    public class ImageModel : BaseModel
    {
        [EntityMap]
        [EntityConversion(typeof(string), typeof(Uri))]
        public Uri Url { get; set; }

        [EntityMap]
        public string Title { get; set; }

        [EntityMap]
        public string SubTitle { get; set; }

        [EntityMap]
        public string Html { get; set; }

        [EntityMap]
        public string Author { get; set; }

        [EntityMap]
        public byte[] Image { get; set; }

        [EntityMap]
        public bool IsLoaded { get; set; }

        [EntityMap]
        public int GalleryId { get; set; }
        public GalleryModel Gallery { get; set; }

        [CallBeforeSave]
        public void SetIds()
        {
            if (Gallery != null)
                GalleryId = Gallery.Id;
        }
    }
}
