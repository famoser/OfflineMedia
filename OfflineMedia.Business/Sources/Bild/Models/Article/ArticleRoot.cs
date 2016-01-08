using System.Collections.Generic;

namespace OfflineMedia.Business.Sources.Bild.Models.Article
{
    public class ArticleRoot
    {
        public string kicker { get; set; }
        public string headline { get; set; }
        public string subline { get; set; }
        public string author { get; set; }
        public List<Text> text { get; set; }
        public LeadElement leadElement { get; set; }
        public Outbrain outbrain { get; set; }
        public TopicPage topicPage { get; set; }
        public int adStatus { get; set; }
        public bool isLiveArticle { get; set; }
        public string sharingURL { get; set; }
        public string doctype { get; set; }
        public string docpath { get; set; }
        public string ivwcode2 { get; set; }
        public Keywords wtChannels { get; set; }
        public string pubDate { get; set; }
        public int coremediaId { get; set; }
        public string __nodeType__ { get; set; }
    }
}