using System;
using System.Linq;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.UnitTests.Helpers.Models;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers
{
    public class AssertHelper
    {
        public static bool TestFeedArticleProperties(ArticleModel article, LogEntry entry)
        {
            var res = true;
            res &= TestStringNotEmptyProperty(article.LogicUri, "LogicUri", entry);
            res &= TestStringNotEmptyProperty(article.PublicUri, "PublicUri", entry);
            res &= TestStringNotEmptyProperty(article.Title, "Title", entry);
            res &= TestStringNotEmptyProperty(article.SubTitle, "SubTitle", entry);
            res &= TestStringNotEmptyProperty(article.Teaser, "Teaser", entry);

            res &= TestDateTimeNotEmptyProperty(article.PublishDateTime, "PublishDateTime", entry);
            res &= TestDateTimeNotEmptyProperty(article.DownloadDateTime, "DownloadDateTime", entry);

            return res;
        }

        private static bool TestStringNotEmptyProperty(string propertyValue, string propertyName, LogEntry entry)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "Property is empty: " + propertyName
                });
                return false;
            }
            return true;
        }

        private static bool TestDateTimeNotEmptyProperty(DateTime propertyValue, string propertyName, LogEntry entry)
        {
            if (propertyValue == DateTime.MinValue || propertyValue == DateTime.MaxValue)
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "DateTime is not set: " + propertyName
                });
                return false;
            }
            if (propertyValue < DateTime.Now - TimeSpan.FromDays(200) ||
                propertyValue > DateTime.Now - TimeSpan.FromDays(200))
            {

                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "DateTime is probably wrong (value: " + propertyValue + "): " + propertyName
                });
                return false;
            }
            return true;
        }

        public static bool AssertFullArticleProperties(ArticleModel article, LogEntry entry)
        {
            var res = TestFeedArticleProperties(article, entry);
            Assert.IsNotNull(article.Author, "No Author for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Content, "No Content for " + GetArticleDescription(article));
            Assert.IsTrue(article.Content.Any(), "0 Content for " + GetArticleDescription(article));
            Assert.IsNotNull(article.Themes, "No Themes for " + GetArticleDescription(article));
            Assert.IsTrue(article.Themes.Any(), "0 Themes for " + GetArticleDescription(article));
            return res;
        }

        public static string GetArticleDescription(ArticleModel articleModel)
        {
            return articleModel.Title + " - " + articleModel.SubTitle + "; " + articleModel.LogicUri;
        }
    }
}
