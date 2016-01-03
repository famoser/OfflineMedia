using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums;

namespace TamediaLinkAggregator.Models
{
    class ShortSourceConfigurationModel
    {
        public Guid Guid { get; set; }

        public string SourceNameLong { get; set; }
        public string SourceNameShort { get; set; }

        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public SourceEnum Source { get; set; }

        public List<ShortFeedConfigurationModel> FeedConfigurationModels { get; set; }
    }
}
