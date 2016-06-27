namespace OfflineMedia.Data.Entities.Contents
{
    public class GalleryContentEntity : EntityIdBase
    {
        public string Url { get; set; }
        public int TextContentId { get; set; }
        public int LoadingState { get; set; }
    }
}
