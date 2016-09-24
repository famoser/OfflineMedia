using System;
using System.Collections.Generic;
using Famoser.OfflineMedia.Business.Enums;
using Famoser.OfflineMedia.Business.Models.Base;
using Famoser.OfflineMedia.Business.Models.Progress;
using Famoser.OfflineMedia.Business.Services.Interfaces;

namespace Famoser.OfflineMedia.View.Services
{
    public class ProgressService : BaseModel, IProgressService
    {
        private readonly Dictionary<ProgressType, ProgressModel> _progressModels = new Dictionary<ProgressType, ProgressModel>();
        public void StartIndeterminate(ProgressType type)
        {
            if (!_progressModels.ContainsKey(type))
                _progressModels[type] = new ProgressModel(type);
            ActiveProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Start(ProgressType type, int maxValue)
        {
            if (!_progressModels.ContainsKey(type))
                _progressModels[type] = new ProgressModel(type, maxValue);
            else
                _progressModels[type].OverwriteMaxValue(maxValue);
        }

        public void IncreaseMaxValue(ProgressType type, int amount)
        {
            if (_progressModels.ContainsKey(type))
                _progressModels[type].IncrementMaxValue(amount);
        }

        public void Incremenent(ProgressType type)
        {
            if (_progressModels.ContainsKey(type))
            {
                if (_progressModels[type].ActiveValue < _progressModels[type].MaxValue || _progressModels[type].IsIndeterminate)
                    _progressModels[type].IncrementProgress();
            }
        }

        public void Stop(ProgressType type)
        {
            if (_progressModels.ContainsKey(type))
                _progressModels.Remove(type);
            ActiveProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        public ProgressModel GetActiveProgress()
        {
            if (_progressModels.ContainsKey(ProgressType.Feed))
                return _progressModels[ProgressType.Feed];
            if (_progressModels.ContainsKey(ProgressType.Article))
                return _progressModels[ProgressType.Article];
            if (_progressModels.ContainsKey(ProgressType.Image))
                return _progressModels[ProgressType.Image];

            return null;
        }

        public event EventHandler ActiveProgressChanged;
    }
}
