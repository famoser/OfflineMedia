using System.Collections.Generic;

namespace OfflineMedia.Business.Newspapers.Stern.Models
{
    public class Credits
    {
        public string author { get; set; }
    }
    
    public class Head
    {
        public string kicker { get; set; }
        public string headline { get; set; }
        public string link { get; set; }
        public string teaser { get; set; }
        public string displayDate { get; set; }
        public Credits credits { get; set; }
        public string adKeywords { get; set; }
        public string adZone { get; set; }
        public int adTopBannerId { get; set; }
        public int adInterstitialId { get; set; }
        public int bottomPositionId { get; set; }
    }

    public class Content2
    {
        public string type { get; set; }
        public object content { get; set; }
        public string href { get; set; }
        public List<Content2> children { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public object content { get; set; }
        public string href { get; set; }
        public List<Content2> children { get; set; }
    }

    public class Related
    {
        public string articleType { get; set; }
        public string contentId { get; set; }
        public string timestamp { get; set; }
        public string kicker { get; set; }
        public string headline { get; set; }
        public List<Image> images { get; set; }
    }

    public class Image2
    {
        public string src { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }

    public class SternArticle
    {
        public string contentId { get; set; }
        public string lastModified { get; set; }
        public string module { get; set; }
        public string articleType { get; set; }
        public Head head { get; set; }
        public List<Content> content { get; set; }
        public List<Related> related { get; set; }
        public List<Image2> images { get; set; }
    }
}
