using System.Collections.Generic;
using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Zeit.Models
{
    [XmlRoot(ElementName = "cluster")]
    public class Cluster
    {
        [XmlElement(ElementName = "region")]
        public List<Region> Region { get; set; }
        [XmlAttribute(AttributeName = "area")]
        public string Area { get; set; }
        [XmlAttribute(AttributeName = "kind")]
        public string Kind { get; set; }
        [XmlAttribute(AttributeName = "kind_title")]
        public string Kind_title { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "visible_mobile")]
        public string Visible_mobile { get; set; }
    }
}