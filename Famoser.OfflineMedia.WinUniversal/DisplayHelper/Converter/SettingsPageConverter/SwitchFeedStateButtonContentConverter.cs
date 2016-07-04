using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Famoser.OfflineMedia.Business.Models;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.SettingsPageConverter
{
    public class SwitchFeedStateButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var feed = value as FeedModel;
            if (feed != null)
            {
                if (!feed.Source.ActiveFeeds.Contains(feed))
                    return "off";
            }
            return "on";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
