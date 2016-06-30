using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Models.Base;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class ThemeModel : BaseIdModel
    {
        [EntityMap]
        public string DisplayName { get; set; }
        [EntityMap]
        public string SaveName { get; set; }
    }
}
