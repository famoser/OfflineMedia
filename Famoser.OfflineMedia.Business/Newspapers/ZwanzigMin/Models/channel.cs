using System.Collections.Generic;
using System.Xml.Serialization;

namespace Famoser.OfflineMedia.Business.Newspapers.ZwanzigMin.Models
{
    // ReSharper disable InconsistentNaming
    public class channel
    {
        [XmlElement("item")]
        public List<item> item;
    }
}
