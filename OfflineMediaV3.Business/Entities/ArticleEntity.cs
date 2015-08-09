using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMediaV3.Business.Enums;
using SQLite.Net.Attributes;

namespace OfflineMediaV3.Business.Entities
{
    public class ArticleEntity
    {
        [PrimaryKey]
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

        public DateTime ChangeDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
