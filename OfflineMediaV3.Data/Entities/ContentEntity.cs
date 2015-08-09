namespace OfflineMediaV3.Data.Entities
{
    public class ContentEntity : EntityBase
    {
        public int ArticleId { get; set; }

        public int ContentType { get; set; }
        public int Order { get; set; }

        public string Html { get; set; }
        public int ImageId { get; set; }
        public int GalleryId { get; set; }
    }
}
