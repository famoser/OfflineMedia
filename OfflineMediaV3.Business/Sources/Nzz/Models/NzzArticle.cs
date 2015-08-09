using System;

namespace OfflineMediaV3.Business.Sources.Nzz.Models
{
    public class NzzArticle
    {
        public string path, title, subTitle, leadText, webUrl, shortWebUrl;
        public DateTime publicationDateTime;
        public bool isBreakingNews;
        public NzzBody[] body;
        public NzzAuthor[] authors;
        public NzzRelatedArticle[] relatedArticles;
        public NzzRelatedContent[] relatedContent;
        public NzzLeadImage leadImage;
    }
}
