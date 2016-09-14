using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Famoser.OfflineMedia.Business.Enums.Models;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.ArticlePageConverter
{
    public class LoadingStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var loadingState = (LoadingState)value;
            switch (loadingState)
            {
                case LoadingState.Loaded:
                    return "fertig geladen";
                case LoadingState.Loading:
                    return "wird geladen";
                case LoadingState.LoadingFailed:
                    return "laden fehlgeschlagen";
                case LoadingState.New:
                    return "neu";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
