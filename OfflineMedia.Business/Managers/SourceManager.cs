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
        private static ObservableCollection<SourceModel> _allSources = new ObservableCollection<SourceModel>();
        private static ObservableCollection<SourceModel> _activeSources = new ObservableCollection<SourceModel>();

        public static void AddSource(SourceModel model, bool isActive = false)
        {
            if (!_allSources.Contains(model))
                _allSources.Add(model);
            if (isActive && !_activeSources.Contains(model))
                _activeSources.Add(model);
            if (!isActive && _activeSources.Contains(model))
                _activeSources.Remove(model);
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
            return _activeSources;
        }

        public static ObservableCollection<SourceModel> GetAllSources()
        {
            return _allSources;
        }
    }
}
