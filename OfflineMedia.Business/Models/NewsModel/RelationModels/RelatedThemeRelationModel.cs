using Famoser.SqliteWrapper.Attributes;

namespace OfflineMedia.Business.Models.NewsModel.RelationModels
{
    public class RelatedThemeRelationModel
    {
        [EntityMap]
        public int Theme1Id { get; set; }

        [EntityMap]
        public int Theme2Id { get; set; }
    }
}
