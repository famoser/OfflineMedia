using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace OfflineMedia.Business.Helpers.Text
{
    public class HtmlConverter
    {
        public static ObservableCollection<ParagraphModel> HtmlToParagraph(string html)
        {
            //todo
            cm.Html = WebUtility.HtmlDecode(cm.Html);
            cm.Html = cm.Html.Replace("&nbsp;", " ");
        } 
    }
}
