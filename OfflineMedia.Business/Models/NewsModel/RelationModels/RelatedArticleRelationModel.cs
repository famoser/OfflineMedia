using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Models.Base;

namespace OfflineMedia.Business.Models.NewsModel.RelationModels
{
    public class RelatedArticleRelationModel : BaseModel
    {
        [EntityMap]
        public int Article1Id { get; set; }

        [EntityMap]
        public int Article2Id { get; set; }
    }
}
