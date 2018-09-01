using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;


namespace Esmart.Framework.Redis
{
    public static class RedisHelperConfig
    {
        private static string[] connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RedisQueue"].ConnectionString.Split(':');
        public static long QueueLength
        {
            get
            {
                return Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["QueueLength"]);
            }
        }

        public static string IP
        {
            get
            {
                return connectionString[0];
            }
        }

        public static int Port
        {
            get { return int.Parse(connectionString[1]); }
        }
    }

    public sealed class RedisQueueHelper 
    {
        private long _db = 0;
        public RedisQueueHelper(long db = 0)
        {
            _db = db;
        }

        private RedisClient GetClient()
        {
            RedisClient client = new RedisClient(RedisHelperConfig.IP, RedisHelperConfig.Port, null, this._db);
            return client;
        }

        /// <summary>
        /// 入队操作
        /// </summary>
        /// <param name="key">队例名称</param>
        /// <param name="value">队例的值</param>
        /// <returns></returns>
        public long LPush(string key, string value)
        {
            using (RedisClient client = GetClient())
            {
                long lNum = client.LLen(key);
                if (lNum < RedisHelperConfig.QueueLength)
                {
                    byte[] Val = Encoding.UTF8.GetBytes(value);
                    lNum = client.LPush(key, Val);
                }
                return lNum;
            }
        }

        /// <summary>
        /// 出队操作
        /// </summary>
        /// <param name="key">队例名称</param>
        /// <returns></returns>
        public string RPop(string key)
        {

            using (RedisClient client = GetClient())
            {
                byte[][] Val = client.BRPop(key, 0);
                if (Val != null)
                {
                    string value = Encoding.UTF8.GetString(Val[1]);
                    return value;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取队列长度
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public long GetQueueLen(string key)
        {
            using (RedisClient client = GetClient())
            {
                long lNum = client.LLen(key);
                return lNum;
            }
        }

    }

}
