using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Enums.Models;

namespace OfflineMedia.Business.Models.NewsModel.ContentModels
{
    public class TextModel
    {
        public string Text { get; set; }
        public TextType TextType { get; set; }
        public List<TextModel> Children { get; set; }
    }
}
