using System;

namespace Esmart.Framework.MyCache
{
    public class MyCacheManager
    {
        public static ICache<TKey, TResult> GetCache<TKey, TResult>(string cacheKey,Func<TKey,TResult> createvalueFactory)
        {
            return CacheHolder.Instance.GetCache(cacheKey, createvalueFactory);
        }
    }
}
