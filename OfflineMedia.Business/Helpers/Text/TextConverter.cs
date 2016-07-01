using System.Collections.Generic;
using System.Collections.ObjectModel;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace OfflineMedia.Business.Helpers.Text
{
    public class TextConverter
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
    }
}
