using System.Collections.Generic;
using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
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
