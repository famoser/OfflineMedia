using System.Collections.Generic;
using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace Famoser.OfflineMedia.Business.Helpers.Text
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
