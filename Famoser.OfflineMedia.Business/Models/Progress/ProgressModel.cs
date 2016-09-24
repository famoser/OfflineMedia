using Famoser.OfflineMedia.Business.Enums;
using Famoser.OfflineMedia.Business.Models.Base;

namespace Famoser.OfflineMedia.Business.Models.Progress
{
    public class ProgressModel : BaseModel
    {
        private readonly ProgressType _progressType;

        public ProgressModel(ProgressType progressType)
        {
            IsIndeterminate = true;
            _progressType = progressType;
        }

        public ProgressModel(ProgressType progressType, int maxValue)
        {
            _progressType = progressType;
            MaxValue = maxValue;
        }

        public bool IsIndeterminate { get; }

        private int _maxValue;
        public int MaxValue
        {
            get { return _maxValue; }
            private set { Set(ref _maxValue, value); }
        }

        private int _activeValue;
        public int ActiveValue
        {
            get { return _activeValue; }
            private set { Set(ref _activeValue, value); }
        }

        public string Description
        {
            get
            {
                switch (_progressType)
                {
                    case ProgressType.Article:
                        return "Artikel werden heruntergeladen...";
                    case ProgressType.Feed:
                        return "Feeds werden heruntergeladen...";
                    case ProgressType.Image:
                        return "Bilder werden heruntergeladen...";
                }
                return "bitte warten...";
            }
        }

        public void IncrementProgress()
        {
            ActiveValue++;
        }

        public void IncrementMaxValue(int amount)
        {
            MaxValue++;
        }

        public void OverwriteMaxValue(int maxValue)
        {
            MaxValue = maxValue;
        }
    }
}
