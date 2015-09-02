using System;

namespace OfflineMediaV3.Business.Models.Configuration
{
    public class FeedConfigurationModel : SimpleSettingModel
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public SourceConfigurationModel SourceConfiguration { get; set; }
    }
}
