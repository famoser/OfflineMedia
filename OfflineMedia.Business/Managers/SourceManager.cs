using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfflineMedia.Business.Models;

namespace OfflineMedia.Business.Managers
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
        }

        public static void AddFeed(FeedModel fm, SourceModel source, bool isActive = false)
        {
            fm.Source = source;
            fm.Source.AllFeeds.Add(fm);
            if (isActive)
                fm.Source.ActiveFeeds.Add(fm);
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
                fm.Source.ActiveFeeds.Add(fm);
            else if (!isActive && fm.Source.ActiveFeeds.Contains(fm))
                fm.Source.ActiveFeeds.Remove(fm);
        }

        public static void SetSourceActiveState(SourceModel sm, bool isActive)
        {
            if (isActive && !ActiveSources.Contains(sm))
                ActiveSources.Add(sm);
            else if (!isActive && ActiveSources.Contains(sm))
                ActiveSources.Remove(sm);
        }
    }
}
