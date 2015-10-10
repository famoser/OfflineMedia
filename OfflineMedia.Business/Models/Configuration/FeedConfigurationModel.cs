namespace OfflineMedia.Business.Models.Configuration
{
    public class FeedConfigurationModel : SimpleSettingModel
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public SourceConfigurationModel SourceConfiguration { get; set; }
    }
}
