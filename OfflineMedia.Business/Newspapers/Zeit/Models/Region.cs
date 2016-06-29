using System.Collections.Generic;
using System.Xml.Serialization;

namespace OfflineMedia.Business.Newspapers.Zeit.Models
{
    [XmlRoot(ElementName = "region")]
    public class Region
    {
        [XmlElement(ElementName = "container")]
        public List<Container> Container { get; set; }
        [XmlAttribute(AttributeName = "area")]
        public string Area { get; set; }
        [XmlAttribute(AttributeName = "automatic_type")]
        public string Automatic_type { get; set; }
        [XmlAttribute(AttributeName = "count")]
        public string Count { get; set; }
        [XmlAttribute(AttributeName = "hide-dupes")]
        public string Hidedupes { get; set; }
        [XmlAttribute(AttributeName = "kind")]
        public string Kind { get; set; }
        [XmlAttribute(AttributeName = "read_more_url")]
        public string Read_more_url { get; set; }
        [XmlAttribute(AttributeName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "visible_mobile")]
        public string Visible_mobile { get; set; }
    }
}