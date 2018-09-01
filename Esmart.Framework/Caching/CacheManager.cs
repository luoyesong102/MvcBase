using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using Esmart.Framework.Model;
using CSRedis;

namespace Esmart.Framework.Caching
{
    public class CacheManager
    {
        static ConnectionMultiplexer _redis = null;

        /// <summary>
        /// redius锁定
        /// </summary>
        static object locaRedis = new object();

        /// <summary>
        /// MenmberCache锁定
        /// </summary>
        static object localMenmberCache = new object();

        /// <summary>
        /// 创建Redis缓存
        /// </summary>
        /// <returns></returns>
        public static ICache CreateRedisCache()
        {
            if (ConstantDefine.NotUserCache)
            {
                return new NotImpCache();
            }
            try
            {

                if (_redis == null)
                {
                    lock (locaRedis)
                    {
                        _redis = GetConnectString();
                    }
                }

                return new RedisCache(_redis, connectString);
            }
            catch
            {
                ConstantDefine.SetCacheUnUse();
                return new NotImpCache();
            }
        }


        private static Dictionary<string, int> GetSettings()
        {
            string address = GlobalConfig.RedisCacheAddress;

            Dictionary<string, int> dic = new Dictionary<string, int>();

            if (string.IsNullOrEmpty(address))
            {
                ConstantDefine.SetCacheUnUse();

                return dic;
            }

            var addressArray = address.Split(',');

            foreach (var addressA in addressArray)
            {
                if (!string.IsNullOrEmpty(addressA))
                {
                    var endponints = addressA.Split(':');

                    if (endponints.Length == 2)
                    {
                        dic.Add(endponints[0], Convert.ToInt32(endponints[1]));
                    }
                    else
                    {
                        dic.Add(endponints[0], 26379);
                    }
                }
            }
            return dic;
        }


        static string connectString;
        private static ConnectionMultiplexer GetConnectString()
        {
            ConnectionMultiplexer connect = null;

            var dic = GetSettings();

            string master = GlobalConfig.SentinelMaster ?? "mymaster";

            foreach (var keyValue in dic)
            {

                using (var sentinel = new RedisSentinelClient(keyValue.Key, keyValue.Value))
                {
                    try
                    {
                        var masterInfo = sentinel.Master(master);

                        if (masterInfo != null)
                        {
                            connect = ConnectionMultiplexer.Connect(string.Format("{0}:{1},allowAdmin=true", masterInfo.Ip, masterInfo.Port));

                            connectString = string.Format("{0}:{1}", masterInfo.Ip, masterInfo.Port);

                            return connect;
                        }
                    }
                    catch
                    {

                    }
                }

                if (connect == null)
                {

                    connect = ConnectionMultiplexer.Connect(GlobalConfig.RedisCacheAddress);

                   if (connect != null)
                   {
                       return connect;
                   }
                }
            }
            return connect;

        }


        public static ICache CreateNetCache()
        {
            return new NetCache();
        }

        /// <summary>
        /// 创建系统默认的cache （微软的cache）
        /// </summary>
        /// <returns></returns>
        public static ICache CreateCache()
        {
            return new NetCache();
        }


        ~CacheManager()
        {
            if (_redis != null && _redis.IsConnected)
            {
                _redis.Dispose();
                _redis = null;
            }
           
        }
    }
}
