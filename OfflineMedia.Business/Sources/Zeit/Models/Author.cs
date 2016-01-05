using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Zeit.Models
{
    [XmlRoot(ElementName = "author")]
    public class Author
    {
        [XmlElement(ElementName = "display_name")]
        public string Display_name { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "publication-date")]
        public string Publicationdate { get; set; }
        [XmlAttribute(AttributeName = "expires")]
        public string Expires { get; set; }
        [XmlElement(ElementName = "location")]
        public string Location { get; set; }
    }
}