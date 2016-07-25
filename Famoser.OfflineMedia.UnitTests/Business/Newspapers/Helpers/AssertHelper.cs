using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using Famoser.OfflineMedia.UnitTests.Helpers.Models;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers
{
    public class AssertHelper
    {
        public static bool TestFeedArticleProperties(ArticleModel article, LogEntry entry)
        {
            var res = true;
            //no logic uri needed as all info is in feed
            if (!Is20MinArticle(article))
                res &= TestStringNotEmptyProperty(article.LogicUri, "LogicUri", entry);
            res &= TestStringNotEmptyProperty(article.PublicUri, "PublicUri", entry);
            res &= TestStringNotEmptyProperty(article.Title, "Title", entry);

            //exclude tamedia from subtitles as they do not provide any
            //exclude tamedia as not provided
            if (!IsTamediaArticle(article) && !IsPostillionArticle(article))
                res &= TestStringNotEmptyProperty(article.SubTitle, "SubTitle", entry);

            //not provided by positillion & blick, nzz is added later (but not to all)
            if (!IsPostillionArticle(article) && !IsBlickArticle(article) && !IsNzzArticle(article))
                res &= TestStringNotEmptyProperty(article.Teaser, "Teaser", entry);
            res &= TestDateTimeNotEmptyProperty(article.DownloadDateTime, "DownloadDateTime", entry);

            return res;
        }

        public static bool TestFullArticleProperties(ArticleModel article, LogEntry entry)
        {
            var res = TestFeedArticleProperties(article, entry);

            if (!IsBlickArticle(article) && !IsPostillionArticle(article) && !IsZeitArticle(article)) //publish date time may be from 2015 or even earlier, blick cause bad, postillion cause they publish some old articles always
                res &= TestDateTimeNotEmptyProperty(article.PublishDateTime, "PublishDateTime", entry);

            res &= TestStringNotEmptyProperty(article.Author, "Author", entry);
            res &= TestBooleanFalseProperty(article.IsRead, "IsRead", entry);
            res &= TestBooleanFalseProperty(article.IsFavorite, "IsFavorite", entry);
            res &= TestNotEmptyCollection(article.Themes, "Themes", entry);
            res &= TestNotEmptyCollection(article.Content, "Content", entry);
            res &= TestContentModels(article.Content, entry);
            res &= TestForCorrectValue(article.LoadingState, LoadingState.Loaded, "LoadingState", entry);
            return res;
        }

        private static bool IsTamediaArticle(ArticleModel article)
        {
            return article.LogicUri != null && article.LogicUri.StartsWith("http://mobile2.");
        }

        private static bool IsBlickArticle(ArticleModel article)
        {
            return article.LogicUri != null && article.LogicUri.StartsWith("http://www.blick.ch/");
        }

        private static bool Is20MinArticle(ArticleModel article)
        {
            return article.PublicUri != null && (article.PublicUri.Contains("20min.ch") || article.PublicUri.Contains("friday-magazine.ch"));
        }

        private static bool IsNzzArticle(ArticleModel article)
        {
            return article.PublicUri != null && (article.PublicUri.Contains("nzz.ch"));
        }

        private static bool IsPostillionArticle(ArticleModel article)
        {
            return article.PublicUri != null && article.PublicUri.StartsWith("http://www.der-postillon.com/");
        }

        private static bool IsZeitArticle(ArticleModel article)
        {
            return article.PublicUri != null && article.PublicUri.StartsWith("http://zeit.de");
        }

        private static bool TestStringNotEmptyProperty(string propertyValue, string propertyName, LogEntry entry)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "Property is empty: " + propertyName,
                    IsFaillure = true
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
                    Content = "DateTime is not set: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            if (propertyValue < DateTime.Now - TimeSpan.FromDays(200) ||
                propertyValue > DateTime.Now + TimeSpan.FromDays(200))
            {

                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "DateTime is probably wrong (value: " + propertyValue + "): " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestBooleanFalseProperty(bool propertyValue, string propertyName, LogEntry entry)
        {
            if (propertyValue)
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "Boolean is true which should be false: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestNotEmptyCollection<T>(IEnumerable<T> propertyValue, string propertyName, LogEntry entry)
        {
            if (!propertyValue.Any())
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "Collection is empty which should not be empty: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestForCorrectValue(object propertyValue, object expectedValue, string propertyName, LogEntry entry)
        {
            if (!propertyValue.Equals(expectedValue))
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "Wrong value. Expected: " + expectedValue + "; Value: " + propertyValue + " in property " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestTextModels(List<TextModel> textModels, LogEntry entry, bool root = true)
        {
            if (root && !textModels.Any())
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "No text entries! Collection is empty",
                    IsFaillure = true
                });
                return false;
            }

            var successfull = true;
            foreach (var textModel in textModels)
            {
                if (string.IsNullOrEmpty(textModel.Text) && !textModel.Children.Any())
                {
                    entry.LogEntries.Add(new LogEntry()
                    {
                        Content = "Text model is empty!",
                        IsFaillure = true
                    });
                    successfull = false;
                }
                successfull &= TestTextModels(textModel.Children, entry, false);
            }
            return successfull;
        }

        private static bool TestParagraphModels(ObservableCollection<ParagraphModel> paragraphs, LogEntry entry, bool root = true)
        {
            if (root && !paragraphs.Any())
            {
                entry.LogEntries.Add(new LogEntry()
                {
                    Content = "No paragraphs! Collection is empty",
                    IsFaillure = true
                });
                return false;
            }

            var successfull = true;
            foreach (var paragraphModel in paragraphs)
            {
                successfull &= TestTextModels(paragraphModel.Children, entry);
            }
            return successfull;
        }

        private static bool TestContentModels(IEnumerable<BaseContentModel> baseContentModels, LogEntry entry)
        {
            var res = true;
            foreach (var baseContentModel in baseContentModels)
            {
                var successfull = true;
                var logEntry = new LogEntry()
                {
                    Content = "ContentModel of type "
                };
                if (baseContentModel is ImageContentModel)
                {
                    var image = (ImageContentModel)baseContentModel;
                    logEntry.Content += "image";
                    successfull &= TestStringNotEmptyProperty(image.Url, "Url", logEntry);
                }
                else if (baseContentModel is TextContentModel)
                {
                    var text = (TextContentModel)baseContentModel;
                    logEntry.Content += "text";
                    successfull &= TestParagraphModels(text.Content, logEntry);
                }
                else if (baseContentModel is GalleryContentModel)
                {
                    var gallery = (GalleryContentModel)baseContentModel;
                    logEntry.Content += "gallery";
                    if (gallery.Text != null)
                        successfull &= TestParagraphModels(gallery.Text.Content, logEntry);
                    successfull &= TestNotEmptyCollection(gallery.Images, "gallery.Images", logEntry);
                    foreach (var imageContentModel in gallery.Images)
                    {
                        successfull &= TestStringNotEmptyProperty(imageContentModel.Url, "Url", logEntry);
                    }
                }
                else
                {
                    logEntry.Content += "unknown! typeof: " + baseContentModel.GetType().FullName;
                    logEntry.IsFaillure = true;
                    successfull = false;
                }

                if (successfull)
                    logEntry.Content += ": all OK";

                res &= successfull;
                entry.LogEntries.Add(logEntry);
            }
            return res;
        }
    }
}
