namespace OfflineMedia.Business.Newspapers.Nzz.Models
{
    public class NzzRelatedContent
    {
        public string type, guid, path, source, caption, mimeType;
        public NzzImage[] images;
        public NzzArticle article;
        public bool isReady; 
    }
}
