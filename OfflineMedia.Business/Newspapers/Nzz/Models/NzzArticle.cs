using System;

namespace OfflineMedia.Business.Newspapers.Nzz.Models
{
    public class NzzArticle
    {
        public string path, title, subTitle, leadText, webUrl, shortWebUrl, agency;
        public string[] departments;
        public DateTime publicationDateTime;
        public bool isBreakingNews;
        public NzzBody[] body;
        public NzzAuthor[] authors;
        public NzzRelatedArticle[] relatedArticles;
        public NzzRelatedContent[] relatedContent;
        public NzzLeadImage leadImage;
    }
}
