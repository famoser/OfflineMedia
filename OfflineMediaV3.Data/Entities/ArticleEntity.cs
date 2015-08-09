using System;

namespace OfflineMediaV3.Data.Entities
{
    public class ArticleEntity : EntityBase
    {
        public string Uri { get; set; }

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Teaser { get; set; }

        public string LogicUri { get; set; }

        public string Author { get; set; }

        public int State { get; set; }
        public bool IsFavorite { get; set; }

        public DateTime PublicationTime { get; set; }

        public int LeadImageId { get; set; }

        public string FeedId { get; set; }
        public string SourceId { get; set; }
    }
}
