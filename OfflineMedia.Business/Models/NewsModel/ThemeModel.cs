using Famoser.OfflineMedia.Business.Models.Base;
using Famoser.SqliteWrapper.Attributes;

namespace Famoser.OfflineMedia.Business.Models.NewsModel
{
    public class ThemeModel : BaseIdModel
    {
        [EntityMap]
        public string Name { get; set; }
        [EntityMap]
        public string NormalizedName { get; set; }
    }
}
