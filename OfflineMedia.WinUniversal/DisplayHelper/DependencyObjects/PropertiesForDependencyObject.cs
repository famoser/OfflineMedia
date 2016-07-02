using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;

namespace OfflineMedia.WinUniversal.DisplayHelper.DependencyObjects
{
    public class Properties : DependencyObject
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached("Content", typeof(ObservableCollection<BaseContentModel>), typeof(RichTextBlock), new PropertyMetadata(null, ContentChanged));

        public ObservableCollection<BaseContentModel> Content
        {
            get { return (ObservableCollection<BaseContentModel>)this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.RegisterAttached("FontSize", typeof(int), typeof(RichTextBlock), new PropertyMetadata(null, ContentChanged));

        public string FontSize
        {
            get { return (string)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty CustomFontFamilyProperty =
            DependencyProperty.RegisterAttached("CustomFontFamily", typeof(string), typeof(RichTextBlock), new PropertyMetadata(null, ContentChanged));

        public string CustomFontFamily
        {
            get { return (string)this.GetValue(CustomFontFamilyProperty); }
            set { this.SetValue(CustomFontFamilyProperty, value); }
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as RichTextBlock;
            if (richTextBox != null)
            {
                
            }
        }
    }
}
