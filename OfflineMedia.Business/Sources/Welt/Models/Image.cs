using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.Welt.Models
{
    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlElement(ElementName = "link")]
        public string Link { get; set; }
        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }
}
