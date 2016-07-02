using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticlePageConverter
{
    public class ThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var themes = value as List<ThemeModel>;
            if (themes != null && themes.Any())
            {
                var res = themes[0].Name;
                for (int i = 1; i < themes.Count; i++)
                {
                    res += ", " + themes[i].Name;
                }
                return res;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
