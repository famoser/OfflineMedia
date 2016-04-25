using System;
using GalaSoft.MvvmLight;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class BaseModel: ObservableObject
    {
        [EntityMap]
        [EntityPrimaryKey]
        public int Id { get; set; }

        [EntityMap]
        public DateTime CreateDate { get; set; }

        [EntityMap]
        public DateTime ChangeDate { get; set; }
    }
}
