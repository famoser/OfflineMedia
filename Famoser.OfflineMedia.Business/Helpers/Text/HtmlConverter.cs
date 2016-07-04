using System;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using HtmlAgilityPack;

namespace Famoser.OfflineMedia.Business.Helpers.Text
{
    public class HtmlConverter
    {
        public static HtmlConverter CreateOnce()
        {
            return new HtmlConverter();
        }

        public ObservableCollection<ParagraphModel> HtmlToParagraph(string html)
        {
            html = CleanHtml(html);
            ConfigureTags(html);

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

        private string CleanHtml(string html)
        {
            return html;
        }

        private void ConfigureTags(string html)
        {
            var allTitles = new[] { "h1", "h2", "h3", "h4", "h5", "h6" };
            for (int i = 0; i < allTitles.Length; i++)
            {
                if (html.Contains("<" + allTitles[i]))
                {
                    Array.Resize(ref _primaryTitles, i + 1);
                    for (int j = 0; j < i + 1; j++)
                    {
                        _primaryTitles[j] = allTitles[j];
                    }

                    Array.Resize(ref _secondaryTitles, allTitles.Length - (i + 1));
                    for (int j = i + 1; j < allTitles.Length; j++)
                    {
                        _secondaryTitles[j - (i + 1)] = allTitles[i + 1];
                    }
                    return;
                }
            }
        }

        private string[] _primaryTitles = { };
        private string[] _secondaryTitles = { };
        private readonly string[] _texts = { "p" };
        private readonly string[] _quotes = { "blockquote" };
        private ParagraphModel ParseParagraph(HtmlNode node)
        {
            var model = new ParagraphModel();

            if (_primaryTitles.Any(predicate => predicate == node.Name))
                model.ParagraphType = ParagraphType.Title;
            else if (_secondaryTitles.Any(predicate => predicate == node.Name))
                model.ParagraphType = ParagraphType.SecondaryTitle;
            else if (_texts.Any(predicate => predicate == node.Name))
                model.ParagraphType = ParagraphType.Text;
            else if (_quotes.Any(predicate => predicate == node.Name))
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

        private TextModel ParseText(HtmlNode parentNode)
        {
            var model = new TextModel();
            var texts = new[] { "h1", "h2", "h3", "h4", "p" };
            var bolds = new[] { "b", "strong", "em" };
            var cursives = new[] { "i" };
            var underlines = new[] { "u" };
            var hyperlink = new[] { "a" };

            if (parentNode.ChildNodes.Count() == 1)
            {
                var node = parentNode.ChildNodes.FirstOrDefault();
                if (node.NodeType == HtmlNodeType.Text)
                {
                    model.Text = node.InnerText.Trim();
                    return model;
                }
            }

            if (texts.Any(predicate => predicate == parentNode.Name))
                model.TextType = TextType.Normal;
            else if (bolds.Any(predicate => predicate == parentNode.Name))
                model.TextType = TextType.Bold;
            else if (cursives.Any(predicate => predicate == parentNode.Name))
                model.TextType = TextType.Cursive;
            else if (underlines.Any(predicate => predicate == parentNode.Name))
                model.TextType = TextType.Underline;
            else if (hyperlink.Any(predicate => predicate == parentNode.Name))
                model.TextType = TextType.Hyperlink;
            else
                return null;

            foreach (var node in parentNode.ChildNodes)
            {
                var tm = ParseText(node);
                if (tm != null)
                    model.Children.Add(tm);
            }


            return !string.IsNullOrEmpty(model.Text) || model.Children.Any() ? model : null;
        }
    }
}
