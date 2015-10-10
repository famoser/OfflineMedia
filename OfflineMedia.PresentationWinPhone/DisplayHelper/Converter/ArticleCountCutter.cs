using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.DisplayHelper.Converter
{
    public class ArticleCountCutter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ObservableCollection<ArticleModel> oc = (ObservableCollection<ArticleModel>)value;
            if (oc.Count > 5)
                return new ObservableCollection<ArticleModel>() { oc[0], oc[1], oc[2], oc[3], oc[4] };
            else
                return oc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
