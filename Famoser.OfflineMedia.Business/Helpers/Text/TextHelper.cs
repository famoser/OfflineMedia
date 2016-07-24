using System.Collections.Generic;
using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace Famoser.OfflineMedia.Business.Helpers.Text
{
    public class TextHelper
    {
        public static TextContentModel TextToTextModel(string text)
        {
            return new TextContentModel()
            {
                Content = new ObservableCollection<ParagraphModel>()
                {
                    new ParagraphModel()
                    {
                        Children = new List<TextModel>()
                        {
                            new TextModel()
                            {
                                Text = text
                            }
                        }
                    }
                }
            };
        }

        public static string NormalizeString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;

            str = str.Replace("\n", " ");
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            return str;
        }

        public static string StripHTML(string str)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, @"<(.|\n)*?>", string.Empty);
        }
    }
}
