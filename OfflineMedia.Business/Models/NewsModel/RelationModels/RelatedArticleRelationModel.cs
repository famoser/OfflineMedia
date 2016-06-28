using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel.NMModels
{
    public class RelatedArticleRelationModel : BaseModel
    {
        [EntityMap]
        public int Article1Id { get; set; }

        [EntityMap]
        public int Article2Id { get; set; }
    }
}
