﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models.NewsModel;

namespace Famoser.OfflineMedia.Business.Managers
{
    public class ThemeManager
    {
        private static readonly ObservableCollection<ThemeModel> AllThemes = new ObservableCollection<ThemeModel>();
        private static readonly ConcurrentDictionary<string, ThemeModel> ThemeDic = new ConcurrentDictionary<string, ThemeModel>();

        public static void AddTheme(ThemeModel model)
        {
            AllThemes.Add(model);
            ThemeDic.TryAdd(model.NormalizedName, model);
        }

        public static ThemeModel TryAddTheme(ThemeModel model)
        {
            AllThemes.Add(model);
            if (ThemeDic.TryAdd(model.NormalizedName, model))
                return model;
            return ThemeDic[model.NormalizedName];
        }

        public static void AddThemes(IEnumerable<ThemeModel> themes)
        {
            foreach (var themeModel in themes)
            {
                AddTheme(themeModel);
            }
        }

        public static ThemeModel TryGetSimilarTheme(string name)
        {
            if (ThemeDic.ContainsKey(name))
                return ThemeDic[name];
            return null;
        }

        public static ObservableCollection<ThemeModel> GetAllThemes()
        {
            return AllThemes;
        }
    }
}
