using System.Xml.Serialization;

namespace OfflineMedia.Business.Newspapers.Welt.Models
{
    [XmlRoot(ElementName = "rss")]
    public class Rss
    {
        [XmlElement(ElementName = "channel")]
        public Channel Channel { get; set; }
        [XmlAttribute(AttributeName = "georss", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Georss { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}