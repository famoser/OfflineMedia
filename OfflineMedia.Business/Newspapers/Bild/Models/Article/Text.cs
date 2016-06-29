using System.Collections.Generic;

namespace OfflineMedia.Business.Newspapers.Bild.Models.Article
{
    public class Text
    {
        public string CDATA { get; set; }
        public string __nodeType__ { get; set; }
        public string headline { get; set; }
        public string kicker { get; set; }
        public Images images { get; set; }
        public string imageURL { get; set; }
        public int? imageWidth { get; set; }
        public int? imageHeight { get; set; }
        public string length { get; set; }
        public string video { get; set; }
        public bool? showAd { get; set; }
        public string source { get; set; }
        public string text { get; set; }
        public string imageURL2 { get; set; }
        public int? imageHeight2 { get; set; }
        public int? imageWidth2 { get; set; }
        public int? inlineAlign { get; set; }
        public string doctype { get; set; }
        public string docpath { get; set; }
        public string ivwcode2 { get; set; }
        public Keywords wtChannels { get; set; }
        public int? coremediaId { get; set; }
        public string imageCaption { get; set; }
        public string altText { get; set; }
        public int? height { get; set; }
        public int? width { get; set; }
        public List<ChildNode> __childNodes__ { get; set; }
    }
}