namespace OfflineMedia.Business.Sources.Bild.Models.Feed
{
    public class Images
    {
        public string __nodeType__ { get; set; }
        public Image portrait { get; set; }
        public Image landscape { get; set; }
        public Image aTeaser { get; set; }
        public Image superATeaser { get; set; }
        public Image banner { get; set; }
    }
}