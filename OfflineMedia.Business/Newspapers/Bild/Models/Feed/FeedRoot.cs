using System.Collections.Generic;

namespace OfflineMedia.Business.Newspapers.Bild.Models.Feed
{
    public class FeedRoot
    {
        public int adStatus { get; set; }
        public string name { get; set; }
        public string doctype { get; set; }
        public string docpath { get; set; }
        public string ivwcode2 { get; set; }
        public int coremediaId { get; set; }
        public string __nodeType__ { get; set; }
        public List<ChildNode> __childNodes__ { get; set; }
    }
}