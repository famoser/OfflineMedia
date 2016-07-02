using System.Xml.Serialization;

namespace Famoser.OfflineMedia.Business.Newspapers.Zeit.Models
{
    [XmlRoot(ElementName = "cp_extra")]
    public class Cp_extra
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
}