using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Data.Entities.Storage.Base;

namespace OfflineMedia.Data.Entities.Storage
{
    public class FeedEntity : StorageEntityBase
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<FeedEntity> Feeds { get; set; }
    }
}
