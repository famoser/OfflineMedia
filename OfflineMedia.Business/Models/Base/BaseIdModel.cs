using Famoser.SqliteWrapper.Models.Interfaces;

namespace OfflineMedia.Business.Models.Base
{
    public class BaseIdModel: BaseModel, ISqliteModel
    {
        private int Id { get; set; }

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
