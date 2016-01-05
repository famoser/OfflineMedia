using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Zeit.Models
{
    [XmlRoot(ElementName = "cp_extra")]
    public class Cp_extra
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
}