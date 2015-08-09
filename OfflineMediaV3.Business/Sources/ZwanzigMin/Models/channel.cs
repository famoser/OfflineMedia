using System.Collections.Generic;
using System.Xml.Serialization;

namespace OfflineMediaV3.Business.Sources.ZwanzigMin.Models
{
    // ReSharper disable InconsistentNaming
    public class channel
    {
        [XmlElement("item")]
        public List<item> item;
    }
}
