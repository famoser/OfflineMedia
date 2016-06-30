using System;
using GalaSoft.MvvmLight;

namespace OfflineMedia.Business.Models.Base
{
    public class BaseGuidModel: ObservableObject
    {
        public Guid Guid { get; set; }
    }
}
