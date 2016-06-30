using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.Base;

namespace OfflineMedia.Business.Models.NewsModel.RelationModels
{
    public class ContentModel : BaseModel
    {
        [EntityMap]
        public int ArticleId { get; set; }
        [EntityMap]
        public int Index { get; set; }

        [EntityMap]
        [EntityConversion(typeof(int), typeof(ContentType))]
        public ContentType ContentType { get; set; }
        [EntityMap]
        public int ContentId { get; set; }
    }
}
