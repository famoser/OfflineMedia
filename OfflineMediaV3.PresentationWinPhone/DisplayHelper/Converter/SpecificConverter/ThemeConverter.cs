using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using OfflineMediaV3.Business.Models.NewsModel;

namespace OfflineMediaV3.DisplayHelper.Converter.SpecificConverter
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
