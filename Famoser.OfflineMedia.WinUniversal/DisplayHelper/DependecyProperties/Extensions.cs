using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;

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
            element.SetValue(CustomContentProperty, value);
        }

        public static int GetCustomFontSize(UIElement element)
        {
            return (int)element.GetValue(CustomContentProperty);
        }


        public static readonly DependencyProperty CustomFontFamilyProperty =
    DependencyProperty.Register("CustomFontFamily", typeof(string), typeof(Extensions), new PropertyMetadata(null, CustomFontFamilyChanged));

        public static void SetCustomFontFamily(UIElement element, string value)
        {
            element.SetValue(CustomFontFamilyProperty, value);
        }

        public static string GetCustomFontFamily(UIElement element)
        {
            return (string)element.GetValue(CustomFontFamilyProperty);
        }

        private static void CustomContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as RichTextBlock;
            var newValue = e.NewValue as ObservableCollection<BaseContentModel>;
            if (richTextBox != null)
            {
            }
        }

        private static void CustomFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as RichTextBlock;
            var newValue = (int)e.NewValue;
            if (richTextBox != null)
            {

            }
        }

        private static void CustomFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = d as RichTextBlock;
            var newValue = e.NewValue as string;
            if (richTextBox != null)
            {

            }
        }
    }
}
