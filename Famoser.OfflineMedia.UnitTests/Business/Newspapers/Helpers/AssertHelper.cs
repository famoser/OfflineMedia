using System.Linq;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers
{
    public class AssertHelper
    {
        public static void AssertFeedArticleProperties(ArticleModel article)
        {
            Assert.IsNotNull(article.LogicUri, "No LogicUri for " + GetArticleDescription(article));
            Assert.IsNotNull(article.PublicUri, "No PublicUri for " + GetArticleDescription(article));
            Assert.IsNotNull(article.PublishDateTime, "No PublicationTime for " + GetArticleDescription(article));
            Assert.IsNotNull(article.DownloadDateTime, "No DownloadDateTime for " + GetArticleDescription(article));
            Assert.IsNotNull(article.SubTitle, "No SubTitle for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Title, "No Title for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Teaser, "No Teaser for " + GetArticleDescription(article));
        }

        public static void AssertFullArticleProperties(ArticleModel article)
        {
            AssertFeedArticleProperties(article);
            Assert.IsNotNull(article.Author, "No Author for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Content, "No Content for " + GetArticleDescription(article));
            Assert.IsTrue(article.Content.Any(), "0 Content for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Themes, "No Themes for " + GetArticleDescription(article));
            Assert.IsTrue(article.Themes.Any(), "0 Themes for " + GetArticleDescription(article));
        }

        public static string GetArticleDescription(ArticleModel articleModel)
        {
            return articleModel.Title + " - " + articleModel.SubTitle + "; " + articleModel.LogicUri;
        }
    }
}
