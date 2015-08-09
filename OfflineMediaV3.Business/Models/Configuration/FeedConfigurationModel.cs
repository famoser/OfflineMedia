using System;
using GalaSoft.MvvmLight;

namespace OfflineMediaV3.Business.Models.Configuration
{
    public class FeedConfigurationModel : GuidSettingModel
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Guid SourceGuid { get; set; }
    }
}
