using System;

namespace OfflineMedia.Business.Newspapers.Nzz.Models
{
    public class NzzRelatedArticle
    {
        public string path, title, subTitle, teaser;
        public DateTime publicationDateTime;
        public NzzLeadImage leadImage;
        public bool isBreakingNews, hasGallery, isReportage;
        public string[] departments;
    }
}
