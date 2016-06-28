using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel.NMModels
{
    public class ThemeArticleRelationModel : BaseModel
    {
        [EntityMap]
        public int ThemeId { get; set; }

        [EntityMap]
        public int ArticleId { get; set; }
    }
}
