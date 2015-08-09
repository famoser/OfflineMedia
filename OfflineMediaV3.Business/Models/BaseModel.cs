using System;
using GalaSoft.MvvmLight;
using OfflineMediaV3.Common.Framework;

namespace OfflineMediaV3.Business.Models
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
