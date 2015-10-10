using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OfflineMedia.Business.Enums.Models;
using OfflineMedia.Business.Models.NewsModel;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.SourceTests.Helpers
{
    public class AssertHelper : SingletonBase<AssertHelper>
    {
        public void AssertFeedArticleProperties(ArticleModel article)
        {
            Assert.IsNotNull(article.LogicUri, "No LogicUri for " + GetArticleDescription(article));
            Assert.IsNotNull(article.PublicUri, "No PublicUri for " + GetArticleDescription(article));
            Assert.IsNotNull(article.PublicationTime, "No PublicationTime for " + GetArticleDescription(article));
            Assert.IsNotNull(article.SubTitle, "No SubTitle for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Title, "No Title for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Teaser, "No Teaser for " + GetArticleDescription(article));
            AssertArticleLinkedCorrectly(article);
        }

        public void AssertFullArticleProperties(ArticleModel article)
        {
            AssertFeedArticleProperties(article);
            Assert.IsNotNull(article.Author, "No Author for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Content, "No Content for " + GetArticleDescription(article));
            Assert.IsTrue(article.Content.Any(), "0 Content for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Themes, "No Themes for " + GetArticleDescription(article));
            Assert.IsTrue(article.Themes.Any(), "0 Themes for " + GetArticleDescription(article));
        }

        public void AssertArticleLinkedCorrectly(ArticleModel article)
        {
            if (article.Content != null)
            {
                foreach (var contentModel in article.Content)
                {
                    Assert.IsTrue(contentModel.Article == article || contentModel.Article == null);
                    if (contentModel.ContentType == ContentType.Gallery)
                    {
                        Assert.IsNotNull(contentModel.Gallery);
                        if (contentModel.Gallery.Images != null)
                            foreach (var imageModel in contentModel.Gallery.Images)
                            {
                                Assert.IsTrue(imageModel.Gallery == contentModel.Gallery);
                            }
                    }
                }
            }
        }

        public string GetArticleDescription(ArticleModel articleModel)
        {
            return articleModel.Title + " - " + articleModel.SubTitle + "; " + articleModel.LogicUri;
        }
    }
}
