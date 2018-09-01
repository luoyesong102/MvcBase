using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;
using Esmart.Framework.Model;

namespace Esmart.Framework.Caching
{
    public class RedisCache : ICache, IDisposable
    {
        private ConnectionMultiplexer _redis = null;

        private string connectString = null;

        internal RedisCache(ConnectionMultiplexer redis, string _connectString)
        {
            _redis = redis;
            connectString = _connectString;
        }


        public string ConnectString { get { return connectString; } }

        public bool Add<T>(string key, T value)
        {
            if (value == null)
            {
                return Remove(key);
            }

            try
            {
                var db = _redis.GetDatabase();
                var json = JsonConvert.SerializeObject(value);
                return db.StringSet(key, json);
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }

            return false;
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            if (expiresAt <= DateTime.Now) return false;

            if (value == null)
            {
                return Remove(key);
            }

            try
            {
                var db = _redis.GetDatabase();
                var json = JsonConvert.SerializeObject(value);
                return db.StringSet(key, json, expiresAt - DateTime.Now);
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
            return false;
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            if (value == null)
            {
                return Remove(key);
            }

            try
            {
                var db = _redis.GetDatabase();
                var json = JsonConvert.SerializeObject(value);
                return db.StringSet(key, json, expiresIn);
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
            return false;
        }

        /// <summary>
        /// 使用此方法需要在连接中配置allowAdmin=true
        /// </summary>
        public void FlushAll()
        {
            try
            {
                var db = _redis.GetDatabase();
                var endPoints = _redis.GetEndPoints();
                foreach (var endPoint in endPoints)
                {
                    var server = _redis.GetServer(endPoint);
                    server.FlushDatabase();
                }
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
        }

        public T Get<T>(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                var value = db.StringGet(key);
                if (value.HasValue)
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
            return default(T);
        }

        public IDictionary<string, Tvalue> GetAll<Tvalue>(IEnumerable<string> keys)
        {

            try
            {
                var db = _redis.GetDatabase();
                var dic = new Dictionary<string, Tvalue>(keys.Count());
                foreach (var key in keys)
                {
                    var value = db.StringGet(key);
                    dic[key] = JsonConvert.DeserializeObject<Tvalue>(value);
                }
                return dic;
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }

            return null;
        }

        public bool Remove(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                return db.KeyDelete(key);
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
            return false;
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                Remove(key);
            }
        }

        public bool Replace<T>(string key, T value)
        {
            return Add(key, value);
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return Add(key, value, expiresAt);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return Add(key, value, expiresIn);
        }

        public bool Set<T>(string key, T value)
        {
            return Add(key, value);

        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return Add(key, value, expiresAt);

        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Add(key, value, expiresIn);
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            foreach (var kv in values)
            {
                Add(kv.Key, kv.Value);
            }
        }


        public void Dispose()
        {
            try
            {
                if (_redis != null)
                {
                    _redis.Dispose();
                }
            }
            catch
            {
                ConstantDefine.SetCacheUnUse();
            }
        }

        public void AddItemToList(string key, string value)
        {
            try
            {
                var db = _redis.GetDatabase();
                var json = JsonConvert.SerializeObject(value);
                db.StringSet(key, json);
                db.ListRightPush(key, value);
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
        }


        public List<string> GetAllItemsFromList(string key)
        {
            try
            {
                var db = _redis.GetDatabase();
                return db.ListRange(key).Select(n => n.ToString()).ToList();
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
            return new List<string>();
        }

        /// <summary>
        /// 删除列表中的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int RemoveItemFromList(string key, string value)
        {
            try
            {
                var db = _redis.GetDatabase();
                return Convert.ToInt32(db.ListRemove(key, value));
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
            return -1;
        }


        public void RemoveItemFromList(string key, List<string> values)
        {
            try
            {
                var db = _redis.GetDatabase();
                foreach (var value in values)
                {
                    db.ListRemove(key, value);
                }
            }
            catch
            {
                if (_redis != null && _redis.IsConnected)
                {
                    _redis.Dispose();
                }
                ConstantDefine.SetCacheUnUse();
            }
        }
    }
}
