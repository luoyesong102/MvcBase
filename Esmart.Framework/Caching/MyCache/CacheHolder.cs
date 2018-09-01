using System;
using System.Collections.Concurrent;
using Esmart.Framework.Messagging;

namespace Esmart.Framework.MyCache
{
    public class CacheHolder:ICacheHolder
    {
        private readonly ConcurrentDictionary<CacheKey, object> _caches = new ConcurrentDictionary<CacheKey, object>();

        private CacheHolder()
        {
 
        }

        public static CacheHolder Instance
        {
            get
            {
               return SingletonProxy<CacheHolder>.Create(() => new CacheHolder());
            }
        }

        class CacheKey : Tuple<string, Type,Type>
        {
            public CacheKey(string cachekey, Type key, Type result)
                : base(cachekey, key, result)
            {
 
            }
        }

        public ICache<TKey, TResult> GetCache<TKey, TResult>(string cacheKey, Func<TKey,TResult> createValueFactory)
        {
            var key = new CacheKey(cacheKey, typeof(TKey), typeof(TResult));
            var cache = _caches.GetOrAdd(key, new Cache<TKey,TResult>(createValueFactory));
            return (Cache<TKey, TResult>)cache;
        }
    }
}
