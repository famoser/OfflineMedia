using OfflineMedia.Common.Framework;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class ThemeModel : BaseModel
    {
        [EntityMap]
        public string Name { get; set; }
    }
}
