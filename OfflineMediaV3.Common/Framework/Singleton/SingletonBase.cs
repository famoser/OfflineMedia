namespace OfflineMediaV3.Common.Framework.Singleton
{
    /// <summary>
    /// Represents a Singleton stored in SingletonManager
    /// </summary>
    public class SingletonBase<T>
        where T : class , new()
    {
        public static T Instance => SingletonManager.Instance.Get<T>();
    }
}
