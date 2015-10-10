using System.Collections.Generic;
using System.Xml.Serialization;

namespace OfflineMedia.Business.Sources.ZwanzigMin.Models
{
    // ReSharper disable InconsistentNaming
    public class channel
    {
        [XmlElement("item")]
        public List<item> item;
    }
}
