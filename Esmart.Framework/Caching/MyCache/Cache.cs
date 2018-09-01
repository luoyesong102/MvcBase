using System;
using System.Collections.Concurrent;

namespace Esmart.Framework.MyCache
{
    public class Cache<TKey, TResult> : ICache<TKey, TResult>
    {

        private readonly ConcurrentDictionary<TKey, TResult> _entries = new ConcurrentDictionary<TKey, TResult>();

        private readonly Func<TKey, TResult> _createvalueFactory;

        public Cache(Func<TKey, TResult> createValuefactory)
        {
            _createvalueFactory = createValuefactory;
        }

        public TResult Get(TKey key)
        {
            var entry = CreateEntry(key, _createvalueFactory);
            return entry;
        }

        public void Set(TKey key, TResult value)
        {
            _entries.AddOrUpdate(key, value, (k, v) => value);
        }

        public void Remove(TKey key)
        {
            TResult result;
            _entries.TryRemove(key, out result);
        }


        private TResult CreateEntry(TKey key, Func<TKey, TResult> createfactory)
        {
            TResult entry;
            if (!_entries.TryGetValue(key, out entry) || entry == null)
            {
                entry = _entries.AddOrUpdate(key,
                    createfactory(key),
                    (k, v) => createfactory(key));
            }
            return entry;
        }

    }
}
