using System.Xml.Serialization;

namespace Famoser.OfflineMedia.Business.Newspapers.Spiegel.Models
{
    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        [XmlElement(ElementName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "pubDate")]
        public string PubDate { get; set; }
        [XmlElement(ElementName = "guid")]
        public string Guid { get; set; }
        [XmlElement(ElementName = "encoded", Namespace = "http://purl.org/rss/1.0/modules/content/")]
        public string Encoded { get; set; }
        [XmlElement(ElementName = "enclosure")]
        public Enclosure Enclosure { get; set; }
    }
}