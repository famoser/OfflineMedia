﻿using System;

namespace OfflineMedia.Data.Entities
{
    public class ArticleEntity : EntityBase
    {
        public string Uri { get; set; }

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Teaser { get; set; }
        public string Author { get; set; }

        public string LogicUri { get; set; }
        public string PublicUri { get; set; }


        public int LoadingState { get; set; }
        
        public DateTime PublishDateTime { get; set; }
        public DateTime DownloadDateTime { get; set; }
    }
}