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

        public static void AddAllSources(IEnumerable<SourceModel> models, bool isActives = false)
        {
            foreach (var sourceModel in models)
            {
                AddSource(sourceModel, isActives);
            }
        }

        public static ObservableCollection<SourceModel> GetActiveSources()
        {
            return ActiveSources;
        }

        public static ObservableCollection<SourceModel> GetAllSources()
        {
            return AllSources;
        }

        public static void MarkFeedActive(FeedModel fm)
        {
            if (!fm.Source.ActiveFeeds.Contains(fm))
                fm.Source.ActiveFeeds.Add(fm);
        }

        public static void MarkFeedInActive(FeedModel fm)
        {
            if (fm.Source.ActiveFeeds.Contains(fm))
                fm.Source.ActiveFeeds.Remove(fm);
        }
    }
}
