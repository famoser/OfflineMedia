using System.Xml.Serialization;

namespace Famoser.OfflineMedia.Business.Newspapers.Zeit.Models
{
    [XmlRoot(ElementName = "container")]
    public class Container
    {
        [XmlElement(ElementName = "block")]
        public Block Block { get; set; }
        [XmlAttribute(AttributeName = "module")]
        public string Module { get; set; }
        [XmlAttribute(AttributeName = "overlay_level")]
        public string Overlay_level { get; set; }
        [XmlAttribute(AttributeName = "text_color")]
        public string Text_color { get; set; }
        [XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "visible_mobile")]
        public string Visible_mobile { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "cp_extra")]
        public Cp_extra Cp_extra { get; set; }
    }
}