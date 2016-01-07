using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Welt.Models
{
    [XmlRoot(ElementName = "enclosure")]
    public class Enclosure
    {
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
        [XmlAttribute(AttributeName = "mime-type")]
        public string Mimetype { get; set; }
    }
}