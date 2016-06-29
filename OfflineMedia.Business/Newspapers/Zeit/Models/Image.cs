using System.Xml.Serialization;

namespace OfflineMedia.Business.Newspapers.Zeit.Models
{
    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "bu")]
        public string Bu { get; set; }
        [XmlElement(ElementName = "copyright")]
        public string Copyright { get; set; }
        [XmlAttribute(AttributeName = "base-id")]
        public string Baseid { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "alt")]
        public string Alt { get; set; }
        [XmlAttribute(AttributeName = "align")]
        public string Align { get; set; }
        [XmlAttribute(AttributeName = "publication-date")]
        public string Publicationdate { get; set; }
        [XmlAttribute(AttributeName = "expires")]
        public string Expires { get; set; }
    }
}
