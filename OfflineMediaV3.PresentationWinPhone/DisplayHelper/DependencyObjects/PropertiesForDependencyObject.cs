using System;
using System.Linq;
using System.Text;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;
using GalaSoft.MvvmLight.Ioc;
using OfflineMediaV3.Business.Enums;
using OfflineMediaV3.Business.Framework;
using OfflineMediaV3.Business.Framework.Logs;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Services;

namespace OfflineMediaV3.DisplayHelper.DependencyObjects
{
    /// <summary>
    /// Usage: 
    /// 1) In a XAML file, declare the above namespace, e.g.:
    ///    xmlns:h2xaml="using:Html2Xaml"
    ///     
    /// 2) In RichTextBlock controls, set or databind the Html property, e.g.:
    ///    <RichTextBlock h2xaml:Properties.Html="{Binding ...}"/>
    ///    or
    ///    <RichTextBlock>
    ///     <h2xaml:Properties.Html>
    ///         <![CDATA[
    ///             <p>This is a list:</p>
    ///             <ul>
    ///                 <li>Item 1</li>
    ///                 <li>Item 2</li>
    ///                 <li>Item 3</li>
    ///             </ul>
    ///         ]]>
    ///     </h2xaml:Properties.Html>
    /// </RichTextBlock>
    /// </summary>
    public class Properties : DependencyObject
    {
        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached("Html", typeof(ContentModel[]), typeof(Properties), new PropertyMetadata(null, HtmlChanged));

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.RegisterAttached("FontSize", typeof(int), typeof(Properties), new PropertyMetadata(null, HtmlChanged));

        public static void SetHtml(DependencyObject obj, ContentModel[] value)
        {
            obj.SetValue(HtmlProperty, value);
        }

        public static ContentModel[] GetHtml(DependencyObject obj)
        {
            return (ContentModel[])obj.GetValue(HtmlProperty);
        }

        public static void SetFontSize(DependencyObject obj, int value)
        {
            obj.SetValue(FontSizeProperty, value);
        }

        public static int GetFontSize(DependencyObject obj)
        {
            return (int)obj.GetValue(FontSizeProperty);
        }

        private static void HtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBlock)
            {
                string html = "";
                ContentModel[] contentmodels = GetHtml(d);
                if (contentmodels == null)
                {
                    html = "<p>Artikel wird geladen...</p>";
                }
                else
                    html = contentmodels.Where(h => h.Type == ContentType.Html).Aggregate(html, (current, item) => current + item.Html);

                int fontSize = GetFontSize(d);
                if (fontSize == 0)
                    return;

                // Get the target RichTextBlock
                RichTextBlock richText = d as RichTextBlock;

                richText.Blocks.Clear();
                // Wrap the value of the Html property in a div and convert it to a new RichTextBlock
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<RichTextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
                sb.AppendLine(Html2XamlConverter.ConvertString2Xaml(html, fontSize));
                sb.AppendLine("</RichTextBlock>");

                string xaml = sb.ToString();
                try
                {
                    RichTextBlock newRichText = (RichTextBlock)XamlReader.Load(xaml);

                    // Move the blocks in the new RichTextBlock to the target RichTextBlock
                    for (int i = newRichText.Blocks.Count - 1; i >= 0; i--)
                    {
                        Block b = newRichText.Blocks[i];
                        newRichText.Blocks.RemoveAt(i);
                        richText.Blocks.Insert(0, b);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.Log(LogLevel.Error, "Html Dependency Object", "XAML Parse error in RichTextBox",ex);
                }
            }
        }
    }
}
