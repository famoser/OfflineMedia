using System.Collections.ObjectModel;
using System.Linq;
using HtmlAgilityPack;
using OfflineMedia.Business.Enums.Models.TextModels;
using OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace OfflineMedia.Business.Helpers.Text
{
    public class HtmlConverter
    {
        public static ObservableCollection<ParagraphModel> HtmlToParagraph(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var paragraphs = new ObservableCollection<ParagraphModel>();

            foreach (var childNode in doc.DocumentNode.ChildNodes)
            {
                var paragraph = ParseParagraph(childNode);
                if (paragraph != null)
                    paragraphs.Add(paragraph);
            }

            return paragraphs;
            // cm.Html = WebUtility.HtmlDecode(cm.Html);
        }

        private static ParagraphModel ParseParagraph(HtmlNode node)
        {
            var model = new ParagraphModel();
            var titles = new[] { "h1", "h2", "h3", "h4" };
            var texts = new[] { "p" };
            var quotes = new[] { "blockquote" };

            if (titles.Any(predicate => predicate == node.Name))
                model.ParagraphType = ParagraphType.Title;
            else if (texts.Any(predicate => predicate == node.Name))
                model.ParagraphType = ParagraphType.Text;
            else if (quotes.Any(predicate => predicate == node.Name))
                model.ParagraphType = ParagraphType.Quote;
            else
                return null;

            var text = ParseText(node);
            if (text != null)
                model.Children.Add(text);

            foreach (var childNode in node.ChildNodes)
            {
                text = ParseText(childNode);
                if (text != null)
                    model.Children.Add(text);
            }
            
            return model;
        }

        private static TextModel ParseText(HtmlNode node)
        {
            var model = new TextModel();
            var texts = new[] { "h1", "h2", "h3", "h4", "p" };
            var bolds = new[] { "b", "strong", "em" };
            var cursives = new[] { "i" };
            var underlines = new[] { "u" };
            var hyperlink = new[] { "a" };

            if (texts.Any(predicate => predicate == node.Name))
                model.TextType = TextType.Normal;
            else if (bolds.Any(predicate => predicate == node.Name))
                model.TextType = TextType.Bold;
            else if (cursives.Any(predicate => predicate == node.Name))
                model.TextType = TextType.Cursive;
            else if (underlines.Any(predicate => predicate == node.Name))
                model.TextType = TextType.Underline;
            else if (hyperlink.Any(predicate => predicate == node.Name))
                model.TextType = TextType.Hyperlink;
            else
                return null;

            if (ParseInnerText(model, node.InnerText))
                return model;

            return null;
        }

        private static bool ParseInnerText(TextModel model, string content)
        {

            //return true if has content (children or text)
            return true;
        }
    }
}
