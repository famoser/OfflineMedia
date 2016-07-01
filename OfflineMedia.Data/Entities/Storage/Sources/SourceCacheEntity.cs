using System;
using System.Collections.Generic;

namespace OfflineMedia.Data.Entities.Storage.Sources
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
