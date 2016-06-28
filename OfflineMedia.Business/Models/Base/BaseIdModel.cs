using System;
using Famoser.SqliteWrapper.Attributes;
using Famoser.SqliteWrapper.Models.Interfaces;
using GalaSoft.MvvmLight;
using OfflineMedia.Data.Repository;

namespace OfflineMedia.Business.Models.NewsModel
{
    public class BaseIdModel: BaseModel, ISqliteModel
    {
        [EntityMap]
        public int Id { get; set; }

        public int GetId()
        {
            return Id;
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
