using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Data.Entities.Storage.Base;
using OfflineMedia.Data.Enums;

namespace OfflineMedia.Data.Entities.Storage
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
