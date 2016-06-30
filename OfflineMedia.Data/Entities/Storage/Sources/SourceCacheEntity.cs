using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMedia.Data.Entities.Storage.Settings
{
    public class SourceCacheEntity
    {
        public SourceCacheEntity()
        {
            IsEnabledDictionary = new Dictionary<Guid, bool>();
        }

        public Dictionary<Guid, bool> IsEnabledDictionary { get; set; }
    }
}
