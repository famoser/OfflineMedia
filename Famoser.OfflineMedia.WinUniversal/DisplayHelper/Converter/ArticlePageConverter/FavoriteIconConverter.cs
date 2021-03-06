﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticlePageConverter
{
    public class FavoriteIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (bool)value;
            if (res)
                return Symbol.UnFavorite;

            return Symbol.Favorite;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
