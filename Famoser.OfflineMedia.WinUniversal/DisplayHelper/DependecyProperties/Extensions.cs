using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Famoser.OfflineMedia.Business.Enums.Models.TextModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.DependecyProperties
{
    public class Extensions
    {
        public static readonly DependencyProperty CustomContentProperty =
    DependencyProperty.Register("CustomContent", typeof(ObservableCollection<BaseContentModel>), typeof(Extensions), new PropertyMetadata(null, CustomContentChanged));

        public static void SetCustomContent(UIElement element, ObservableCollection<BaseContentModel> value)
        {
            element.SetValue(CustomContentProperty, value);
        }

        public static ObservableCollection<BaseContentModel> GetCustomContent(UIElement element)
        {
            return (ObservableCollection<BaseContentModel>)element.GetValue(CustomContentProperty);
        }

        public static readonly DependencyProperty CustomFontSizeProperty =
    DependencyProperty.Register("CustomFontSize", typeof(int), typeof(Extensions), new PropertyMetadata(null, CustomFontSizeChanged));

        public static void SetCustomFontSize(UIElement element, int value)
        {
            element.SetValue(CustomFontSizeProperty, value);
        }

        public static int GetCustomFontSize(UIElement element)
        {
            var val = element.GetValue(CustomFontSizeProperty);
            var value = val as int? ?? 12;
            return value < 10 ? 10 : value;
        }

        public static readonly DependencyProperty CustomFontFamilyProperty =
    DependencyProperty.Register("CustomFontFamily", typeof(string), typeof(Extensions), new PropertyMetadata(null, CustomFontFamilyChanged));

        public static void SetCustomFontFamily(UIElement element, string value)
        {
            element.SetValue(CustomFontFamilyProperty, value);
        }

        public static string GetCustomFontFamily(UIElement element)
        {
            var val = element.GetValue(CustomFontFamilyProperty);
            return val is string ? (string)val : "Segoe UI";
        }

        private static ObservableCollection<BaseContentModel> _displayedCollection;
        private static void CustomContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e = null)
        {
            var richTextBox = d as RichTextBlock;
            var newValue = GetCustomContent(richTextBox);
            var fontSize = GetCustomFontSize(richTextBox);
            var fontFamily = GetCustomFontFamily(richTextBox);
            /*
            if (_displayedCollection != null)
                // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
                _displayedCollection.CollectionChanged -= (sender, e2) => NewValueOnCollectionChanged(sender, e2, d);
            _displayedCollection = newValue;
            if (_displayedCollection != null)
                _displayedCollection.CollectionChanged += (sender, e2) => NewValueOnCollectionChanged(sender, e2, d);
                */

            if (richTextBox != null && newValue != null)
            {
                richTextBox.Blocks.Clear();
                foreach (var baseContentModel in newValue)
                {
                    if (baseContentModel is TextContentModel)
                    {
                        var textContent = (TextContentModel)baseContentModel;
                        foreach (var paragraphModel in textContent.Content)
                        {
                            var paragraph = new Paragraph { FontFamily = new FontFamily(fontFamily) };
                            if (paragraphModel.ParagraphType == ParagraphType.Title)
                            {
                                paragraph.FontSize = fontSize * 1.5;
                                paragraph.Margin = new Thickness(0, fontSize * 2, 0, fontSize);
                            }
                            else if (paragraphModel.ParagraphType == ParagraphType.SecondaryTitle)
                            {
                                paragraph.FontSize = fontSize * 1.2;
                                paragraph.Margin = new Thickness(0, fontSize * 1.5, 0, fontSize);
                            }
                            else
                            {
                                paragraph.FontSize = fontSize;
                                paragraph.Margin = new Thickness(0, fontSize, 0, fontSize);
                            }
                            foreach (var textModel in paragraphModel.Children)
                            {
                                var span = RenderTextContent(textModel);
                                if (span != null)
                                {
                                    paragraph.Inlines.Add(span);
                                }
                            }
                            richTextBox.Blocks.Add(paragraph);
                        }
                    }
                }
            }
        }

        private static void NewValueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs, DependencyObject d)
        {
            CustomContentChanged(d);
        }

        private static Span RenderTextContent(TextModel text)
        {
            if (text.TextType == TextType.Bold)
            {
                var span = new Bold();
                AddInlineChildren(span, text);
                return span;
            }
            else if (text.TextType == TextType.Cursive)
            {
                var span = new Span();
                AddInlineChildren(span, text);
                foreach (var inline in span.Inlines)
                {
                    inline.FontStyle = FontStyle.Italic;
                }
                return span;
            }
            else if (text.TextType == TextType.Hyperlink)
            {
                var span = new Hyperlink()
                {
                    NavigateUri = new Uri(text.Text)
                };
                AddInlineChildren(span, text);
                return span;
            }
            else if (text.TextType == TextType.Underline)
            {
                var span = new Underline();
                AddInlineChildren(span, text);
                return span;
            }
            else//(text.TextType == TextType.Normal)
            {
                var span = new Span();
                span.Inlines.Add(
                    new Run()
                    {
                        Text = text.Text
                    });
                AddInlineChildren(span, text);
                return span;
            }
        }

        private static void AddInlineChildren(Span span, TextModel model)
        {
            if (model.Children != null)
            {
                foreach (var textModel in model.Children)
                {
                    var span2 = RenderTextContent(textModel);
                    if (span2 != null)
                        span.Inlines.Add(span2);
                }
            }
        }

        private static void CustomFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomContentChanged(d, e);
        }

        private static void CustomFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomContentChanged(d, e);
        }
    }
}
