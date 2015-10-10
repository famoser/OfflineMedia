namespace OfflineMedia.Data.Entities
{
    public class ImageEntity : EntityBase
    {
        public int GalleryId { get; set; }

        public string Url { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Html { get; set; }

        public string Author { get; set; }

        public byte[] Image { get; set; }
        public bool IsLoaded { get; set; }
    }
}
