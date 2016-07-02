﻿using System;
using Windows.UI.Xaml.Data;

namespace OfflineMedia.WinUniversal.DisplayHelper.Converter.MyDayConverter
{
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}