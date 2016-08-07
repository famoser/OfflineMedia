using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticleListConverter
{
    public class ByteToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var li = value as ImageContentModel;
            var param = parameter != null ? parameter as string : "";
            var collapsed = param == "invert" ? Visibility.Visible : Visibility.Collapsed;
            var visible = param == "invert" ? Visibility.Collapsed: Visibility.Visible;
            
            if (li?.Image != null && li.Image.Length > 0)
                return visible;
            return collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
