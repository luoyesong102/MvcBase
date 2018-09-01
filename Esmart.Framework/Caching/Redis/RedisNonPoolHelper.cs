using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using Newtonsoft.Json;

//using CoreSystem.Library;

namespace Esmart.Framework.Redis
{
    public class RedisNonPoolHelper
    {
        private static string[] connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RedisReadWrite"].ConnectionString.Split(':');
        public static long AnalyzeQueryLength = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["AnalyzeQueryLength"]);
        public static long LogQueryLength = Convert.ToInt64(System.Configuration.ConfigurationManager.AppSettings["LogQueueLength"]);
        public static string IP = connectionString[0];
        public static int Port = int.Parse(connectionString[1]);
        public TimeSpan timespan = new TimeSpan(5, 0, 0);
    }

    public sealed class RedisNonPoolHelper<T> : RedisNonPoolHelper where T : class
    {
        private long _db = 0;
        public RedisNonPoolHelper(long db = 0)
        {
            _db = db;
        }

        private static RedisClient client = null;
        private RedisClient GetClient(long db = 0)
        {
            if (client == null)
                client = new RedisClient(IP, Port, null, db);
            return client;
        }
        public long LPush(string Key, T model)
        {
            try
            {
                using (RedisClient client = GetClient(_db))
                {
                    long lNum = client.LLen(Key);
                    if (lNum < AnalyzeQueryLength)
                    {
                        string json = JsonConvert.SerializeObject(model);
                        byte[] Val = Encoding.UTF8.GetBytes(json);
                        lNum = client.LPush(Key, Val);
                    }
                    client.Dispose();
                    return lNum;
                }

            }
            catch (Exception ex)
            {
                Log.WriteLog("RedisHelper->LPush(" + Key + "): 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                return 0;
            }
        }
        public long LPushLog(string Key, T model)
        {
            try
            {
                using (RedisClient client = GetClient(_db))
                {
                    long lNum = client.LLen(Key);
                    if (lNum < LogQueryLength)
                    {
                        string json = JsonConvert.SerializeObject(model);
                        byte[] Val = Encoding.UTF8.GetBytes(json);
                        lNum = client.LPush(Key, Val);
                    }
                    client.Dispose();
                    return lNum;
                }

            }
            catch (Exception ex)
            {
                Log.WriteLog("RedisHelper->LPush(" + Key + "): 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                return 0;
            }
        }

        public T RPop(string Key)
        {
            try
            {
                using (RedisClient client = GetClient(_db))
                {
                    byte[][] Val = client.BRPop(Key, 0);
                    //Log.WriteLog("RedisHelper->RPop(" + Key + "): 获取：1", (int)LogType.Track);

                    if (Val != null)
                    {
                        string json = Encoding.UTF8.GetString(Val[1]);
                        //Log.WriteLog("RedisHelper->RPop(" + Key + "): 值：" + json, (int)LogType.Track);

                        T model = JsonConvert.DeserializeObject<T>(json);
                        client.Dispose();
                        return model;
                    }
                    //Log.WriteLog("RedisHelper->RPop(" + Key + "): 获取：3", (int)LogType.Track);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("RedisHelper->RPop(" + Key + "): 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                return null;
            }
            return null;
        }
        /// <summary>
        /// 获取队列长度
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public long GetQueryLen(string Key)
        {
            using (RedisClient client = GetClient(_db))
            {
                long lNum = client.LLen(Key);
                client.Dispose();
                return lNum;
            }
        }

    }
}
