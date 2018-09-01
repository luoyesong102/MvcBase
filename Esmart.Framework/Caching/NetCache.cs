using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Esmart.Framework.Caching
{
    public class NetCache : ICache
    {
        internal NetCache()
        {
        }

        public bool Add<T>(string key, T value)
        {
            HttpRuntime.Cache.Add(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            return true;
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            HttpRuntime.Cache.Add(key, value, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            return true;
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            HttpRuntime.Cache.Add(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, expiresIn, CacheItemPriority.Default, null);
            return true;
        }


        public void FlushAll()
        {
            List<string> list = new List<string>();
            var data = HttpRuntime.Cache.GetEnumerator();
            while (data.MoveNext())
            {
                list.Add(data.Key.ToString());
            }
            foreach (string key in list)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        public T Get<T>(string key)
        {
            var obj = HttpRuntime.Cache.Get(key);
            if (obj != null)
            {
                return (T)obj;
            }
            return default(T);
        }

        public IDictionary<string,T> GetAll<T>(IEnumerable<string> keys)
        {
            Dictionary<string, T> dic = new Dictionary<string, T>();

            foreach (var key in keys)
            {
                var obj = Get<T>(key);

                if (obj != null)
                {
                    dic.Add(key, obj);
                }
               
            }
            return dic;
        }

      

        public bool Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
            return true;
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            foreach(var key  in  keys)
            {
                HttpRuntime.Cache.Remove(key);
            }
        }

        public bool Replace<T>(string key, T value)
        {
            HttpRuntime.Cache[key] = value;
            return true;
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return Add<T>(key, value, expiresAt);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return Add<T>(key, value, expiresIn);
        }

        public bool Set<T>(string key, T value)
        {
            return Add<T>(key, value);
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return Add<T>(key, value, expiresAt);
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Add<T>(key, value, expiresIn);
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            foreach (var value in values)
            {
                Add<T>(value.Key,value.Value);
            }
        }

        public void AddItemToList(string key, string value)
        {
            List<string> list = Get<List<string>>(key);
            if (list == null)
            {
                list = new List<string>();
            }
            list.Add(value);

          
        }

        public List<string> GetAllItemsFromList(string key)
        {
            return Get<List<string>>(key);
        }

        public int RemoveItemFromList(string key, string value)
        {
            List<string> list = Get<List<string>>(key);
            if (list != null)
            {
                list.Remove(value);
            }
            return 1;
        }


        public void RemoveItemFromList(string key, List<string> values)
        {
            foreach (var value in values)
            {
                RemoveItemFromList(key, value);
            }
        }


        public string ConnectString
        {
            get { return string.Empty; }
        }
    }
}
