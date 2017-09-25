using System;
using System.Collections.Generic;
using Famoser.OfflineMedia.Data.Enums;

namespace Famoser.OfflineMedia.Utils.TamediaRefresher.Models.JsonEntities
{
    class SourceEntity
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public Sources Source { get; set; }
        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }
        public List<FeedEntity> Feeds { get; set; }
    }
}
