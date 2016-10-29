using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using Famoser.OfflineMedia.Data.Enums;
using Famoser.OfflineMedia.UnitTests.Helpers.Models;

namespace Famoser.OfflineMedia.UnitTests.Business.Newspapers.Helpers
{
    public class AssertHelper
    {
        private readonly List<string> _definedOnceProperties = new List<string>();

        public bool TestFeedArticleProperties(ArticleModel article, LogEntry entry)
        {
            var res = true;
            //no logic uri needed as all info is in feed
            if (!Is20MinArticle(article))
                res &= TestStringNotEmptyProperty(article.LogicUri, "LogicUri", entry);

            res &= TestStringNotEmptyProperty(article.PublicUri, "PublicUri", entry);
            res &= TestStringNotEmptyProperty(article.Title, "Title", entry);

            //exclude tamedia from subtitles as they do not provide any
            //exclude positillion as not provided
            //exclude nzz as not always provided
            //exclude spiegel
            if (!IsTamediaArticle(article) && !IsPostillionArticle(article) && !IsNzzArticle(article) && !IsSpiegelArticle(article))
                res &= TestStringNotEmptyProperty(article.SubTitle, "SubTitle", entry);
            else if (TestStringNotEmptyProperty(article.SubTitle, "SubTitle") && !_definedOnceProperties.Contains("SubTitle"))
                _definedOnceProperties.Add("SubTitle");

            
            //positillion: not provided
            //blick: added later
            //nzz: added later (but not to all)
            //bild: not all articles have teaser
            //tamedia: not all articles have teaser
            if (!IsPostillionArticle(article) && !IsBlickArticle(article) && !IsNzzArticle(article) && !IsBildArticle(article) && !IsTamediaArticle(article))
                res &= TestStringNotEmptyProperty(article.Teaser, "Teaser", entry);
            else if (TestStringNotEmptyProperty(article.Teaser, "Teaser") && !_definedOnceProperties.Contains("Teaser"))
                _definedOnceProperties.Add("Teaser");

            res &= TestDateTimeNotEmptyProperty(article.DownloadDateTime, "DownloadDateTime", entry);

            return res;
        }

        public bool TestFullArticleProperties(ArticleModel article, LogEntry entry)
        {
            var res = TestFeedArticleProperties(article, entry);

            res &= TestDateTimeNotEmptyProperty(article.PublishDateTime, "PublishDateTime", entry);
            res &= TestStringNotEmptyProperty(article.Author, "Author", entry);
            res &= TestBooleanFalseProperty(article.IsRead, "IsRead", entry);
            res &= TestBooleanFalseProperty(article.IsFavorite, "IsFavorite", entry);
            res &= TestNotEmptyCollection(article.Themes, "Themes", entry);
            res &= TestNotEmptyCollection(article.Content, "Content", entry);
            res &= TestContentModels(article.Content, entry);
            res &= TestForCorrectValue(article.LoadingState, LoadingState.Loaded, "LoadingState", entry);

            if (article.LeadImage != null && !string.IsNullOrWhiteSpace(article.LeadImage.Url))
                _definedOnceProperties.Add("LeadImage");

            return res;
        }

        public bool NotAlwaysDefinedPropertiesCheckOut(Sources source)
        {
            var tr = _definedOnceProperties.Contains("LeadImage");
            if (source == Sources.Nzz)
            {
                tr &= _definedOnceProperties.Contains("SubTitle");
                tr &= _definedOnceProperties.Contains("Teaser");
            }
            //tamedia
            if ((int) source >= 20 && (int) source <= 35)
                tr &= _definedOnceProperties.Contains("Teaser");
            if (source == Sources.Bild)
                tr &= _definedOnceProperties.Contains("Teaser");
            if (source == Sources.Blick)
                tr &= _definedOnceProperties.Contains("Teaser");
            if (source == Sources.BlickAmAbend)
                tr &= _definedOnceProperties.Contains("Teaser");
            return tr;
        }

        private static bool IsTamediaArticle(ArticleModel article)
        {
            return article.LogicUri != null && article.LogicUri.StartsWith("http://mobile2.");
        }

        private static bool IsBlickArticle(ArticleModel article)
        {
            return article.LogicUri != null && article.LogicUri.StartsWith("http://www.blick.ch/");
        }

        private static bool IsBildArticle(ArticleModel article)
        {
            return article.LogicUri != null && article.LogicUri.StartsWith("http://json.bild.de");
        }

        private static bool Is20MinArticle(ArticleModel article)
        {
            return article.PublicUri != null && (article.PublicUri.Contains("20min.ch") || article.PublicUri.Contains("friday-magazine.ch"));
        }

        private static bool IsSpiegelArticle(ArticleModel article)
        {
            return article.PublicUri != null && article.PublicUri.Contains("spiegel.de");
        }

        private static bool IsNzzArticle(ArticleModel article)
        {
            return article.PublicUri != null && article.PublicUri.Contains("nzz.ch");
        }

        private static bool IsPostillionArticle(ArticleModel article)
        {
            return article.PublicUri != null && article.PublicUri.StartsWith("http://www.der-postillon.com/");
        }

        private static bool IsZeitArticle(ArticleModel article)
        {
            return article.PublicUri != null && article.PublicUri.StartsWith("http://zeit.de");
        }

        private static bool TestStringNotEmptyProperty(string propertyValue, string propertyName, LogEntry entry = null)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                entry?.LogEntries.Add(new LogEntry()
                {
                    Content = "Property is empty: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestDateTimeNotEmptyProperty(DateTime propertyValue, string propertyName, LogEntry entry = null)
        {
            if (propertyValue == DateTime.MinValue || propertyValue == DateTime.MaxValue)
            {
                entry?.LogEntries.Add(new LogEntry()
                {
                    Content = "DateTime is not set: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            if (propertyValue < DateTime.Now - TimeSpan.FromDays(2000) ||
                propertyValue > DateTime.Now + TimeSpan.FromDays(1))
            {

                entry?.LogEntries.Add(new LogEntry()
                {
                    Content = "DateTime is probably wrong (value: " + propertyValue + "): " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestBooleanFalseProperty(bool propertyValue, string propertyName, LogEntry entry = null)
        {
            if (propertyValue)
            {
                entry?.LogEntries.Add(new LogEntry()
                {
                    Content = "Boolean is true which should be false: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestNotEmptyCollection<T>(IEnumerable<T> propertyValue, string propertyName, LogEntry entry = null)
        {
            if (!propertyValue.Any())
            {
                entry?.LogEntries.Add(new LogEntry()
                {
                    Content = "Collection is empty which should not be empty: " + propertyName,
                    IsFaillure = true
                });
                return false;
            }
            return true;
        }

        private static bool TestForCorrectValue(object propertyValue, object expectedValue, string propertyName, LogEntry entry = null)
        {
            if (!propertyValue.Equals(expectedValue))
            {
                entry?.LogEntries.Add(new LogEntry()
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
                if (textModel.TextType == TextType.Hyperlink)
                {
                    try
                    {
                        var pars = new Uri(textModel.Text);
                    }
                    catch //i do not care about the exact exception
                    {
                        entry.LogEntries.Add(new LogEntry()
                        {
                            Content = "Uri is invalid: " + textModel.Text,
                            IsFaillure = true
                        });
                        successfull = false;
                    }
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
