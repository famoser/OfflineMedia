using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Base;

namespace Famoser.OfflineMedia.WinUniversal.DisplayHelper.Converter.SettingsPageConverter
{
    public class SwitchStateButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isActive = (bool)value;
            if (isActive)
                return "on";
            return "off";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
