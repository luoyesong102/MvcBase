namespace Esmart.Framework.MyCache
{
    public interface ICache<TKey,TResult>
    {
        TResult Get(TKey key);

        void Set(TKey key,TResult value);

        void Remove(TKey key);
    }
}
