﻿using System.Collections.ObjectModel;
using Famoser.OfflineMedia.Business.Models;

namespace Famoser.OfflineMedia.Business.Managers
{
    public class SourceManager
    {
        private static readonly ObservableCollection<SourceModel> AllSources = new ObservableCollection<SourceModel>();
        private static readonly ObservableCollection<SourceModel> ActiveSources = new ObservableCollection<SourceModel>();
        
        public static void AddSource(SourceModel model, bool isActive = false)
        {
            if (!AllSources.Contains(model))
                AllSources.Add(model);
            if (isActive && !ActiveSources.Contains(model))
                ActiveSources.Add(model);
            if (!isActive && ActiveSources.Contains(model))
                ActiveSources.Remove(model);
            model.IsActive = isActive;
        }

        public static void AddFeed(FeedModel fm, SourceModel source, bool isActive = false)
        {
            fm.Source = source;
            if (isActive)
                fm.Source.ActiveFeeds.Add(fm);
            fm.IsActive = isActive;
        }

        public static ObservableCollection<SourceModel> GetActiveSources()
        {
            return ActiveSources;
        }

        public static ObservableCollection<SourceModel> GetAllSources()
        {
            return AllSources;
        }

        public static void SetFeedActiveState(FeedModel fm, bool isActive)
        {
            if (isActive && !fm.Source.ActiveFeeds.Contains(fm))
            {
                fm.Source.ActiveFeeds.Add(fm);
                SetSourceActiveState(fm.Source, true);
            }
            else if (!isActive && fm.Source.ActiveFeeds.Contains(fm))
            {
                fm.Source.ActiveFeeds.Remove(fm);
                if (fm.Source.ActiveFeeds.Count == 0)
                    SetSourceActiveState(fm.Source, false);
            }
            fm.IsActive = isActive;
        }

        public static void SetSourceActiveState(SourceModel sm, bool isActive)
        {
            if (isActive && !ActiveSources.Contains(sm))
                ActiveSources.Add(sm);
            else if (!isActive && ActiveSources.Contains(sm))
                ActiveSources.Remove(sm);
            sm.IsActive = isActive;
        }

        public static bool GetSourceActiveState(SourceModel model)
        {
            return ActiveSources.Contains(model);
        }

        public static bool GetFeedActiveState(FeedModel model)
        {
            return model.Source.ActiveFeeds.Contains(model);
        }
    }
}
