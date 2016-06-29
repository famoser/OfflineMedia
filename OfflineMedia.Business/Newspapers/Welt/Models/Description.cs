using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Welt.Models
{
    [XmlRoot(ElementName = "description")]
    public class Description
    {
        [XmlElement(ElementName = "i")]
        public string I { get; set; }
    }
}