using System.Collections.Generic;
using Famoser.OfflineMedia.Data.Entities.Storage.Base;

namespace Famoser.OfflineMedia.Data.Entities.Storage.Sources
{
    public class SourceEntity : StorageEntityBase
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Enums.Sources Source { get; set; }
        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public List<FeedEntity> Feeds { get; set; }
    }
}
