using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;
using HtmlAgilityPack;

namespace OfflineMediaV3.DisplayHelper.DependencyObjects
{    /// <summary>
    /// Class to convert Html text string into WinRT-compatible RichTextBlock Xaml.
    /// </summary>
    public static class Html2XamlConverter
    {
        private static Dictionary<string, Dictionary<string, string>> _attributes = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, TagDefinition> Tags;

        public static void GenerateTags(int fontSize)
        {
            Tags = new Dictionary<string, TagDefinition>(){
			{"div", new TagDefinition("<Span{0}>\n", "</Span>", true)},
			{"p", new TagDefinition("<Paragraph Margin=\"0,10,0,0\" FontSize=\"" + fontSize + "\" LineHeight=\"" + (fontSize*1.5) + "\" LineStackingStrategy=\"MaxHeight\"{0}>", "</Paragraph>\n", true)},
			{"h1", new TagDefinition("<Paragraph Margin=\"0,20,0,0\" FontSize=\"" + (fontSize*1.5) + "\" LineHeight=\"" + (fontSize*1.5*1.5) + "\" LineStackingStrategy=\"MaxHeight\" FontWeight=\"Bold\"{0}>", "</Paragraph>\n", true)},
			{"h2", new TagDefinition("<Paragraph Margin=\"0,20,0,0\" FontSize=\"" + (fontSize*1.5) + "\" LineHeight=\"" + (fontSize*1.5*1.5) + "\" LineStackingStrategy=\"MaxHeight\"{0}>", "</Paragraph>\n", true)},
			{"ul", new TagDefinition(ParseList){MustBeTop = true}},
			{"b", new TagDefinition("<Bold{0}>\n")},
			{"i", new TagDefinition("<Italic{0}>\n")},
			{"u", new TagDefinition("<Underline{0}>\n")},
			{"br", new TagDefinition("<LineBreak />\n", "")},
			{"table", new TagDefinition(ParseTable){ MustBeTop = true}},
			{"blockquote", new TagDefinition("<Paragraph FontSize=\"" + fontSize + "\" FontStyle=\"Italic\" TextIndent=\"12\"{0}>", "</Paragraph>\n", true)}
		};
        }

    /// <summary>
    /// Converts Html to Xaml including attributes that can be used to determine the formatting of individual elements.
    /// <example><code>
    /// string Xaml = Html2XamlConverter.Convert2Xaml(html, new Dictionary<string, Dictionary<string, string>> { 
    /// 					{ "p", new Dictionary<string, string> { { "Margin", "0,10,0,0" } } },
    /// 					{ "i", new Dictionary<string, string> { { "Foreground", "#FF663C00"}}}
    /// 					});
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="htmlString">The Html to convert</param>
    /// <param name="tagAttributes">A dictionary that allows you to add attributes to the Xaml being emitted by this method. 
    /// The first key is the Html tag you want to add formatting to. The dictionary associated with that tag allows you to set
    /// multiple attributes and values associated with that Html tag.</param>
    /// <param name="fontSize"></param>
    /// <returns>Xaml markup that can be used as content in a RichTextBlock</returns>
    public static string ConvertString2Xaml(string htmlString, int fontSize)
        {
            GenerateTags(fontSize);
            return Convert2Xaml(htmlString);
        }

        /// <summary>
        /// Converts Html to Xaml.
        /// </summary>
        /// <param name="htmlString">The Html to convert</param>
        /// <returns>Xaml markup that can be used as content in a RichTextBlock</returns>
        private static string Convert2Xaml(string htmlString)
        {
            PopulateAttributes();

            htmlString = CleanHtml(htmlString);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);
            var xamlString = new StringBuilder();

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                ProcessTopNode(xamlString, node, true);
            }


            return xamlString.ToString();
        }

        private static string CleanHtml(string html)
        {
            //clean from html comments
            var temp = System.Text.RegularExpressions.Regex.Replace(html, "<!--*-->", "");

            return temp;
        }

        private static string CleanXaml(string html)
        {
            //replace Special chars
            html = html.Replace("<", "&lt;");
            html = html.Replace(">", "&gt;");
            html = html.Replace("&", "&amp;");
            html = html.Replace("\"", "&quot;");

            return html;
        }

        private static void ProcessTopNode(StringBuilder xamlString, HtmlNode node, bool isTop = false)
        {
            HtmlNode nextNode = null;
            if (!string.IsNullOrWhiteSpace(node.InnerText))
            {
                if (TestTop(node.FirstChild))
                {
                    ProcessTopNode(xamlString, node.FirstChild);
                    return;
                }
                if (Tags.Where(t => t.Value.MustBeTop).Any(m => m.Key.Equals(node.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    nextNode = ProcessNode(xamlString, node, true);
                }
                else
                {
                    WriteBeginningTag(xamlString, Tags["p"]);
                    nextNode = ProcessNode(xamlString, node, true);
                    WriteEndTag(xamlString, Tags["p"]);
                }
            }

            if (nextNode != null)
                ProcessTopNode(xamlString, nextNode);

            if (!isTop && node.NextSibling != null)
            {
                if (TestTop(node.NextSibling))
                    ProcessTopNode(xamlString, node.NextSibling);
                else
                {
                    WriteBeginningTag(xamlString, Tags["p"]);
                    nextNode = ProcessNode(xamlString, node.NextSibling);
                    WriteEndTag(xamlString, Tags["p"]);
                    if (nextNode != null)
                        ProcessTopNode(xamlString, nextNode);
                }
            }
        }

        //private static HtmlNode getNextTopNode(HtmlNode node)
        //{
        //    if (node.NextSibling != null)
        //        if (testTop(node.NextSibling))
        //            return node.NextSibling;
        //    //else
        //    //	return getNextTopNode(node.NextSibling);

        //    if (node.ParentNode != node.OwnerDocument.DocumentNode && node.ParentNode.NextSibling != null)
        //        if (testTop(node.ParentNode.NextSibling))
        //            return node.ParentNode.NextSibling;
        //    //else
        //    //	return getNextTopNode(node.ParentNode.NextSibling);
        //    return null;
        //}

        private static bool TestTop(HtmlNode node)
        {
            if (node == null)
                return false;
            return (Tags.ContainsKey(node.Name) && Tags[node.Name].MustBeTop);
        }

        private static HtmlNode ProcessNode(StringBuilder xamlString, HtmlNode node, bool isTop = false)
        {
            string tagName = node.Name.ToLower();

            HtmlNode top = null;
            if (Tags.ContainsKey(tagName))
            {
                if (Tags[tagName].MustBeTop && !isTop)
                    return node;

                if (Tags[tagName].IsCustom)
                {
                    Tags[tagName].CustomAction(xamlString, node);
                    return null;
                }

                WriteBeginningTag(xamlString, Tags[tagName]);

                if (node.HasChildNodes)
                    top = ProcessNode(xamlString, node.FirstChild);

                WriteEndTag(xamlString, Tags[tagName]);
            }
            else
            {
                if (node.NodeType == HtmlNodeType.Text)
                    xamlString.Append(CleanXaml(node.InnerText));

                if (node.HasChildNodes)
                    top = ProcessNode(xamlString, node.FirstChild);
            }

            if (top == null && node.NextSibling != null && !isTop)
                top = ProcessNode(xamlString, node.NextSibling);

            return top;
        }

        private static void WriteEndTag(StringBuilder xamlString, TagDefinition tag)
        {
            xamlString.Append(tag.EndXamlTag);
        }

        private static void WriteBeginningTag(StringBuilder xamlString, TagDefinition tag)
        {
            string attrs = string.Empty;
            if (tag.Attributes.Count > 0)
                attrs = " " + string.Join(" ", tag.Attributes.Select(a => string.Format("{0}=\"{1}\"", a.Key, a.Value)).ToArray());

            xamlString.Append(string.Format(tag.BeginXamlTag, attrs));
        }

        private static void PopulateAttributes()
        {
            foreach (var attribute in _attributes)
            {
                if (Tags.ContainsKey(attribute.Key))
                    foreach (var attr in attribute.Value)
                        if (!Tags[attribute.Key].Attributes.ContainsKey(attr.Key))
                            Tags[attribute.Key].Attributes.Add(attr.Key, attr.Value);
            }
        }

        private static void ParseList(StringBuilder xamlString, HtmlNode listNode)
        {
            // Yeah, this actually works out okay, though hard-coded margins and diamond symbol kinda suck.
            foreach (var li in listNode.Descendants("li"))
            {
                xamlString.Append("<Paragraph Margin=\"24,0,0,0\" TextIndent=\"-24\"><Run FontFamily=\"Segoe UI Symbol\">&#x2B27;</Run><Span><Run Text=\"  \"/>");
                ProcessNode(xamlString, li.FirstChild);
                xamlString.Append("</Span></Paragraph>");
            }
        }

        private static void ParseTable(StringBuilder xamlString, HtmlNode tableNode)
        {
            // saddle up, this is going to be a bumpy ride! And yes, it IS a bit indirect to 
            // populate a grid programmatically and then use a custom parser. It turned out to be 
            // the easiest of a lot of really bad options.
            xamlString.Append("<InlineUIContainer>");
            var currentRow = 0;
            var maxColumns = 0;
            var tableGrid = new CustomGrid();
            TextBlock caption = null;
            var cap = tableNode.Descendants("caption").FirstOrDefault();
            if (cap != null)
            {
                caption = new TextBlock() { Text = cap.InnerText };
                currentRow += 1;
                tableGrid.Children.Add(caption);
                tableGrid.RowDefinitions.Add(new RowDefinition());
            }

            foreach (var row in tableNode.Descendants("tr"))
            {
                int colMax;
                tableGrid.RowDefinitions.Add(new RowDefinition());
                var currentColumn = 0;
                foreach (var headerCell in row.Descendants("th"))
                {
                    var cell = new TextBlock { FontWeight = Windows.UI.Text.FontWeights.Bold };
                    colMax = SetCellAttributes(currentRow, currentColumn, headerCell, cell);
                    if (colMax > maxColumns)
                        maxColumns = colMax;
                    tableGrid.Children.Add(cell);
                    currentColumn += 1;
                }

                foreach (var cell in row.Descendants("td"))
                {
                    var textCell = new TextBlock();
                    colMax = SetCellAttributes(currentRow, currentColumn, cell, textCell);
                    if (colMax > maxColumns)
                        maxColumns = colMax;
                    tableGrid.Children.Add(textCell);
                    currentColumn += 1;
                }
                currentRow += 1;
            }

            for (int xx = 0; xx <= maxColumns; xx++)
            {
                tableGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            if (maxColumns > 1 && caption != null)
                Grid.SetColumnSpan(caption, maxColumns);

            var attr = new Dictionary<string, string>();
            if (_attributes.ContainsKey("table"))
                attr = _attributes["table"];
            string xaml = tableGrid.GetXaml(attr);
            xamlString.Append(xaml);
            xamlString.Append("</InlineUIContainer>");
        }

        private static int SetCellAttributes(int currentRow, int currentColumn, HtmlNode cellNode, TextBlock cell)
        {
            int rowSpan = cellNode.GetAttributeValue("rowspan", 0);
            int colSpan = cellNode.GetAttributeValue("colspan", 0);
            if (rowSpan > 0)
            {
                Grid.SetRowSpan(cell, rowSpan);
            }
            if (colSpan > 0)
            {
                Grid.SetColumnSpan(cell, colSpan);
            }
            if (currentRow > 0)
            {
                Grid.SetRow(cell, currentRow);
            }
            if (currentColumn > 0)
            {
                Grid.SetColumn(cell, currentColumn);
            }
            cell.Text = cellNode.InnerText;

            return colSpan + currentColumn;
        }
    }
}
