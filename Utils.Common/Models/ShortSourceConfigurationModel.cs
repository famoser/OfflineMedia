﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using OfflineMedia.Business.Enums;

namespace Utils.Common.Models
{
    public class ShortSourceConfigurationModel
    {
        public Guid Guid { get; set; }

        public string SourceNameLong { get; set; }
        public string SourceNameShort { get; set; }

        public string LogicBaseUrl { get; set; }
        public string PublicBaseUrl { get; set; }

        public Sources Source { get; set; }

        public List<ShortFeedConfigurationModel> FeedConfigurationModels { get; set; }
    }
}
