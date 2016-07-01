using OfflineMedia.Business.Models.Configuration.Base;

namespace OfflineMedia.Business.Models.Configuration
{
    public class SelectSettingModel : BaseSettingModel
    {
        public string[] PossibleValues { get; set; }
    }
}
