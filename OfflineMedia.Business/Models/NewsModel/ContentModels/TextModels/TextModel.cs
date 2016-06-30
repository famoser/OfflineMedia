using System.Collections.Generic;
using OfflineMedia.Business.Enums.Models.TextModels;

namespace OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels
{
    public class TextModel
    {
        public string Text { get; set; }
        public TextType TextType { get; set; }
        public List<TextModel> Children { get; set; }
    }
}
