using OfflineMediaV3.Common.Framework;

namespace OfflineMediaV3.Business.Models.NewsModel.NMModels
{
    public class RelatedArticleRelationModel : BaseModel
    {
        [EntityMap]
        public int Article1Id { get; set; }

        [EntityMap]
        public int Article2Id { get; set; }
    }
}
