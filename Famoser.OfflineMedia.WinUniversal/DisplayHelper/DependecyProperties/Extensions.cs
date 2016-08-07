using System;
using System.Collections.Concurrent;
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
using Nito.AsyncEx;

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

        private static readonly AsyncLock UpdateLock = new AsyncLock();
        private static void UpdateCacheAndCheckIfNewValue(RichTextBlock richTextBlock, ObservableCollection<BaseContentModel> newValue)
        {
            var added = false;
            if (!RichTextBlockCache.ContainsKey(richTextBlock))
            {
                RichTextBlockCache.TryAdd(richTextBlock, newValue);
                added = true;
            }

            var oldValue = RichTextBlockCache[richTextBlock];
            if (oldValue != newValue || added)
            {
                NotifyCollectionChangedEventHandler func =
                    (sender, ev) => NewValueOnCollectionChanged(sender, ev, richTextBlock);
                if (oldValue != null && !added)
                    oldValue.CollectionChanged -= func;
                if (newValue != null)
                    newValue.CollectionChanged += func;

                RichTextBlockCache[richTextBlock] = newValue;
            }
        }

        private static readonly ConcurrentDictionary<RichTextBlock, ObservableCollection<BaseContentModel>> RichTextBlockCache = new ConcurrentDictionary<RichTextBlock, ObservableCollection<BaseContentModel>>();
        private static void CustomContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e = null)
        {
            using (UpdateLock.Lock())
            {
                var richTextBlock = d as RichTextBlock;
                if (richTextBlock == null)
                    return;

                var newValue = GetCustomContent(richTextBlock);
                var fontSize = GetCustomFontSize(richTextBlock);
                var fontFamily = GetCustomFontFamily(richTextBlock);

                // for propertychanged events
                UpdateCacheAndCheckIfNewValue(richTextBlock, newValue);

                richTextBlock.Blocks.Clear();
                if (newValue == null)
                    return;

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
                            paragraph.LineHeight = paragraph.FontSize * 1.6;
                            paragraph.TextIndent = 0;

                            foreach (var textModel in paragraphModel.Children)
                            {
                                var span = RenderTextContent(textModel);
                                if (span != null)
                                {
                                    paragraph.Inlines.Add(span);
                                }
                            }
                            richTextBlock.Blocks.Add(paragraph);
                        }
                    }
                }
            }
        }

        private static void NewValueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs, RichTextBlock rtb)
        {
            CustomContentChanged(rtb);
        }

        private static Span RenderTextContent(TextModel text)
        {
            if (text.TextType == TextType.Bold)
            {
                var span = new Bold();
                if (!string.IsNullOrWhiteSpace(text.Text))
                    span.Inlines.Add(new Run()
                    {
                        Text = text.Text
                    });
                AddInlineChildren(span, text);
                foreach (var inline in span.Inlines)
                {
                    inline.FontWeight = FontWeights.Bold;
                }
                return span;
            }
            if (text.TextType == TextType.Cursive)
            {
                var span = new Span();
                if (!string.IsNullOrWhiteSpace(text.Text))
                    span.Inlines.Add(new Run()
                    {
                        Text = text.Text
                    });
                AddInlineChildren(span, text);
                foreach (var inline in span.Inlines)
                {
                    inline.FontStyle = FontStyle.Italic;
                }
                return span;
            }
            if (text.TextType == TextType.Hyperlink)
            {
                var span = new Hyperlink()
                {
                    NavigateUri = new Uri(text.Text)
                };
                AddInlineChildren(span, text);
                return span;
            }
            if (text.TextType == TextType.Underline)
            {
                var span = new Underline();
                if (!string.IsNullOrWhiteSpace(text.Text))
                    span.Inlines.Add(new Run()
                    {
                        Text = text.Text
                    });
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
