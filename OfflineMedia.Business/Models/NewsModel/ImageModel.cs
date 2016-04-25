using System;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
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

        private byte[] _image;
        [EntityMap]
        public byte[] Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        private bool _isLoaded;
        [EntityMap]
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { Set(ref _isLoaded, value); }
        }

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
