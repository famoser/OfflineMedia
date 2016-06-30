using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Models.Base;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class ThemeModel : BaseIdModel
    {
        [EntityMap]
        public string Name { get; set; }
        [EntityMap]
        public string NormalizedName { get; set; }
    }
}
