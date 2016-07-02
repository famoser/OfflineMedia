using Famoser.OfflineMedia.Business.Models.Configuration.Base;

namespace Famoser.OfflineMedia.Business.Models.Configuration
{
    public class SelectSettingModel : BaseSettingModel
    {
        public string[] PossibleValues { get; set; }
    }
}
