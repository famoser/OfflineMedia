using System;
using GalaSoft.MvvmLight;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class BaseGuidModel: ObservableObject
    {
        public Guid Guid { get; set; }
    }
}
