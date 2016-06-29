using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.SqliteWrapper.Attributes;
using OfflineMedia.Business.Models.NewsModel.ContentModels;
using OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace OfflineMedia.Data.Entities.Contents
{
    public class TextContentModel : BaseContentModel
    {
        [EntityMap]
        public string ContentJson { get; set; }
        public ObservableCollection<ParagraphModel> Content { get; set; }
    }
}
