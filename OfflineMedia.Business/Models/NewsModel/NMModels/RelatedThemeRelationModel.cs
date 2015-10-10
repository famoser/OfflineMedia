using OfflineMedia.Common.Framework;

namespace OfflineMedia.Business.Models.NewsModel.NMModels
{
    public class RelatedThemeRelationModel
    {
        [EntityMap]
        public int Theme1Id { get; set; }

        [EntityMap]
        public int Theme2Id { get; set; }
    }
}
