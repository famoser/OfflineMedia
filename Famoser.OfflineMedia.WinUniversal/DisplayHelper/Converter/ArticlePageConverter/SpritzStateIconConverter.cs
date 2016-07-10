﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Famoser.OfflineMedia.View.Enums;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticlePageConverter
{
    public class SpritzStateIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = (SpritzState) value;

            if (res == SpritzState.Finished)
                return Symbol.Stop;
            else if (res == SpritzState.Paused)
                return Symbol.Play;
            else if (res == SpritzState.Ready)
                return Symbol.Play;
            else
                return Symbol.Pause;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}