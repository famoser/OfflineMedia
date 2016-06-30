using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Models.Base;

namespace OfflineMedia.Business.Models.NewsModel.RelationModels
{
    public class ThemeArticleRelationModel : BaseModel
    {
        [EntityMap]
        public int ThemeId { get; set; }

        [EntityMap]
        public int ArticleId { get; set; }
    }
}
