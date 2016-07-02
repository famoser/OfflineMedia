using System;
using GalaSoft.MvvmLight;

namespace Famoser.OfflineMedia.Business.Models.Base
{
    public class BaseGuidModel: ObservableObject
    {
        public Guid Guid { get; set; }
    }
}
