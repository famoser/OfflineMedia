using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;

namespace Famoser.OfflineMedia.Business.Repositories.Base
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

        protected async Task<T> ExecuteSafe<T>(Func<Task<T>> thign)
        {
            try
            {
                return await thign();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return default(T);
        }
    }
}
