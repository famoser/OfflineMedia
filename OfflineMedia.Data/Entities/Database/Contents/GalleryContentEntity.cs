namespace OfflineMedia.Data.Entities.Contents
{
    public class GalleryContentEntity : EntityBase
    {
        public string Url { get; set; }
        public int TextContentId { get; set; }
        public int LoadingState { get; set; }
    }
}
