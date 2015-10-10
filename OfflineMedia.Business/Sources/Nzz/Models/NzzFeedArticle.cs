using System;

namespace OfflineMedia.Business.Sources.Nzz.Models
{
    public class NzzFeedArticle
    {
        public string path, title, subTitle, teaser;
        public string[] departments;
        public bool isBreakingNews, hasGallery, isReportage;
        public DateTime publicationDateTime;
        public NzzLeadImage leadImage;
    }
}
