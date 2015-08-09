using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfflineMediaV3.Business.Framework.Singleton
{
    /// <summary>
    /// Represents a Singleton stored in SingletonManager
    /// </summary>
    public class SingletonBase<T>
        where T : class , new()
    {
        public static T Instance
        {
            get { return SingletonManager.Instance.Get<T>(); }
        }
    }
}
