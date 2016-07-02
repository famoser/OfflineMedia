using System.Collections.Generic;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;

namespace Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels
{
    public class ParagraphModel
    {
        public ParagraphType ParagraphType { get; set; }
        public List<TextModel> Children { get; set; }
    }
}
