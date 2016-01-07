using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Welt.Models
{
    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "guid")]
        public string Guid { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        [XmlElement(ElementName = "category")]
        public string Category { get; set; }
        [XmlElement(ElementName = "pubDate")]
        public string PubDate { get; set; }
        [XmlElement(ElementName = "enclosure")]
        public Enclosure Enclosure { get; set; }
        [XmlElement(ElementName = "description")]
        public Description Description { get; set; }
    }
}