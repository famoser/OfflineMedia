using System.Collections.Generic;

namespace Famoser.OfflineMedia.Business.Newspapers.Bild.Models.Feed
{
    public class ChildNode
    {
        public string name { get; set; }
        public string __nodeType__ { get; set; }
        public List<ChildNode> __childNodes__ { get; set; }
        public List<string> layouts { get; set; }
        public string linkURL { get; set; }
        public string targetType { get; set; }
        public string doctype { get; set; }
        public string docpath { get; set; }
        public string ivwcode2 { get; set; }
        public Keywords wtChannels { get; set; }
        public int? coremediaId { get; set; }

        
        public string headline { get; set; }
        public string kicker { get; set; }
        public Images images { get; set; }
        public string imageURL { get; set; }
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string length { get; set; }
        public string video { get; set; }
        public string validUntil { get; set; }
        public bool showAd { get; set; }
        public string source { get; set; }
        public string text { get; set; }
        public string imageURL2 { get; set; }
        public int imageHeight2 { get; set; }
        public int imageWidth2 { get; set; }

        
        public string title { get; set; }
        public string teaserImageURL { get; set; }
        public string teaserImageURL2 { get; set; }
        public int renderText { get; set; }
        public string klub { get; set; }
        public int? teaserImageWidth { get; set; }
        public int? teaserImageHeight { get; set; }
    }
}