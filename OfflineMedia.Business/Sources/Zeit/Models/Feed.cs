using System.Collections.Generic;
using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Zeit.Models
{
    [XmlRoot(ElementName = "feed")]
    public class Feed
    {
        [XmlElement(ElementName = "cluster")]
        public List<Cluster> Cluster { get; set; }
    }
}