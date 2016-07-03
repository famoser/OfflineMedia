using System;
using Famoser.SqliteWrapper.Entities;

namespace Famoser.OfflineMedia.Data.Entities.Database
{
    public class ArticleEntity : BaseEntity
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Teaser { get; set; }
        public string Author { get; set; }
        public bool IsRead { get; set; }
        public bool IsFavorite { get; set; }

        public string LogicUri { get; set; }
        public string PublicUri { get; set; }


        public int LoadingState { get; set; }
        
        public DateTime PublishDateTime { get; set; }
        public DateTime DownloadDateTime { get; set; }
    }
}
