using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;

namespace OfflineMedia.Business.Repositories.Base
{
    public class BaseRepository
    {
        protected T ExecuteSafe<T>(Func<T> thign)
        {
            try
            {
                return thign();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return default(T);
        }
    }
}
