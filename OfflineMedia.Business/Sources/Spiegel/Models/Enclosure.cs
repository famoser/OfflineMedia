using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Spiegel.Models
{
    [XmlRoot(ElementName = "enclosure")]
    public class Enclosure
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }
}