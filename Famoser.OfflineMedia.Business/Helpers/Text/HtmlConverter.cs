using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;
using HtmlAgilityPack;

namespace Famoser.OfflineMedia.Business.Helpers.Text
{
    public class HtmlConverter
    {
        private string _baseUrl;

        public HtmlConverter(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public static HtmlConverter CreateOnce(string baseUrl)
        {
            return new HtmlConverter(baseUrl);
        }

        public ObservableCollection<ParagraphModel> HtmlToParagraph(string html)
        {
            if (html == null)
                return new ObservableCollection<ParagraphModel>();

            html = CleanHtml(html);
            ConfigureTags(html);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var paragraphs = new ObservableCollection<ParagraphModel>();

            foreach (var childNode in doc.DocumentNode.ChildNodes)
            {
                var paragraph = ParseParagraph(childNode);
                if (paragraph != null && paragraph.Children.Count > 0)
                    paragraphs.Add(paragraph);
            }

            return paragraphs;
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
                        _secondaryTitles[j - (i + 1)] = allTitles[j];
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
            if (string.IsNullOrWhiteSpace(node.InnerHtml))
                return null;

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

            foreach (var childNode in node.ChildNodes)
            {
                var text = ParseText(childNode);
                if (text != null)
                {
                    CollapseModelsIfNecessary(text, new List<TextType>());
                    model.Children.Add(text);
                }
            }

            return model;
        }

        private void CollapseModelsIfNecessary(TextModel model, List<TextType> knownTextTypes)
        {
            while (model.Children.Count == 1 && knownTextTypes.Contains(model.TextType))
            {
                model.Text = model.Children[0].Text;
                model.TextType = model.Children[0].TextType;
                model.Children = model.Children[0].Children;
            }

            knownTextTypes.Add(model.TextType);
            foreach (var textModel in model.Children)
            {
                CollapseModelsIfNecessary(textModel, knownTextTypes);
            }
        }

        private TextModel ParseText(HtmlNode parentNode)
        {
            var model = new TextModel();
            var texts = new[] { "h1", "h2", "h3", "h4", "p" };
            var bolds = new[] { "b", "strong", "em" };
            var cursives = new[] { "i" };
            var underlines = new[] { "u" };
            var hyperlink = new[] { "a" };

            if (!parentNode.ChildNodes.Any() && parentNode.NodeType == HtmlNodeType.Text)
            {
                model.Text = TextHelper.NormalizeString(TextHelper.StripHtml(parentNode.InnerText));
                if (string.IsNullOrWhiteSpace(model.Text))
                    return null;
                return model;
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
            {
                model.TextType = TextType.Hyperlink;
                model.Text = TextHelper.NormalizeString(parentNode.Attributes["href"]?.Value);

                if (string.IsNullOrWhiteSpace(model.Text))
                    model.TextType = TextType.Normal;
                else
                {
                    if (model.Text.StartsWith("www"))
                        model.Text = "http://" + model.Text;

                    if (!model.Text.StartsWith("http://"))
                    {
                        if (model.Text.StartsWith("/"))
                            model.Text = _baseUrl + model.Text.Substring(1);
                        else
                            model.Text = _baseUrl + model.Text;
                    }

                    if (!Uri.IsWellFormedUriString(model.Text, UriKind.Absolute))
                    {
                        //todo: do additional repair stuff
                        //parse utf8 caracters like: http://www.ragnar%C3%B6k-spektakel.ch
                        model.Text = _baseUrl;
                    }
                }
            }
            else
                return null;

            //shortcut for once node stuff
            if (parentNode.ChildNodes.Count() == 1 && parentNode.ChildNodes.FirstOrDefault().NodeType == HtmlNodeType.Text && model.TextType != TextType.Hyperlink)
            {
                model.Text = TextHelper.NormalizeString(parentNode.ChildNodes.FirstOrDefault().InnerText.Trim());
                if (string.IsNullOrWhiteSpace(model.Text))
                    return null;
                return model;
            }

            foreach (var node in parentNode.ChildNodes)
            {
                var tm = ParseText(node);
                if (tm != null)
                    model.Children.Add(tm);
            }

            if (model.TextType == TextType.Hyperlink && model.Children.Count == 0)
                return null;

            return !string.IsNullOrEmpty(model.Text) || model.Children.Any() ? model : null;
        }
    }
}
