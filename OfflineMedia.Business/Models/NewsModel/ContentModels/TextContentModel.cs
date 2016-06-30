using System.Collections.ObjectModel;
using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace OfflineMedia.Business.Models.NewsModel.ContentModels
{
    public class TextContentModel : BaseContentModel
    {
        [EntityMap]
        public string ContentJson { get; set; }
        public ObservableCollection<ParagraphModel> Content { get; set; }
    }
}
