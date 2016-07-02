using System.Collections.Generic;
using Famoser.OfflineMedia.Data.Entities.Storage.Base;

namespace Famoser.OfflineMedia.Data.Entities.Storage.Sources
{
    public class FeedEntity : StorageEntityBase
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<FeedEntity> Feeds { get; set; }
    }
}
