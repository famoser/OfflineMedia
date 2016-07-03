using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using Famoser.SqliteWrapper.Attributes;

namespace Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels
{
    public class TextContentModel : BaseContentModel
    {
        [EntityMap]
        public string ContentJson { get; set; }
        public ObservableCollection<ParagraphModel> Content { get; set; }
    }
}
