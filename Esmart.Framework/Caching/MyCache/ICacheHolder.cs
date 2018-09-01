using System;

namespace Esmart.Framework.MyCache
{
    public interface ICacheHolder 
    {
        ICache<TKey, TResult> GetCache<TKey, TResult>(string cacheKey,Func<TKey,TResult> createValueFactory);
    }
}
