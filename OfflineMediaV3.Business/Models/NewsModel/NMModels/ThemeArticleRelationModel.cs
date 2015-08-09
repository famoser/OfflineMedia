using OfflineMediaV3.Common.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel.NMModels
{
    public class ThemeArticleRelationModel : BaseModel
    {
        [EntityMap]
        public int ThemeId { get; set; }

        [EntityMap]
        public int ArticleId { get; set; }
    }
}
