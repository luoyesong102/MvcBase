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

    public sealed class RedisHelper<T> where T : class
    {
        public bool Set(string key, T model)
        {
            bool res = false;
            using (var client = RedisManager.GetClient)
            {
                //var client = redisClient.GetTypedClient<T>();
                try
                {
                    res = client.Set<T>(key, model);
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Set: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    res = false;
                }
            }
            return res;
        }

        public bool Set(string key, string value)
        {
            bool res = false;
            using (var client = RedisManager.GetClient)
            {
                //var client = redisClient.GetTypedClient<T>();
                try
                {
                    res = client.Set(key, value);
                }
                catch (Exception ex)
                {
                   Log.WriteLog("Set-string-timespan: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    res = false;
                }
            }
            return res;
        }

        public bool Set(string key, string value, TimeSpan timespan)
        {
            bool res = false;
            using (var client = RedisManager.GetClient)
            {
                //var client = redisClient.GetTypedClient<T>();
                try
                {
                    res = client.Set(key, value, timespan);
                }
                catch (Exception ex)
                {
                   Log.WriteLog("Set-string-timespan: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    res = false;
                }
            }
            return res;
        }

        public bool Set(string key, T model, TimeSpan timespan)
        {
            bool res = false;
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    res = client.Set<T>(key, model, timespan);
                }
                catch (Exception ex)
                {
                   Log.WriteLog("Set-Time: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    res = false;
                }
            }
            return res;
        }

        public bool SetList(string key, IList<T> list)
        {
            bool res = false;
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    res = client.Set<IList<T>>(key, list);
                }
                catch (Exception ex)
                {
                   Log.WriteLog("SetList: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    res = false;
                }
            }
            return res;
        }

        public T Get(string key)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    T model = client.Get<T>(key);
                    return model;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("Get: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    return null;
                }
            }
        }

        //public string GetString(string key)
        //{
        //    using (var client = RedisManager.GetClient)
        //    {
        //        try
        //        {
        //            string strResult = client.Get<string>(key);
        //            return strResult;
        //        }
        //        catch (Exception ex)
        //        {
        //           Log.WriteLog("GetString: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
        //            return null;
        //        }
        //    }
        //}

        //public IDictionary<string, T> GetList(string[] key)
        //{
        //    using (var client = RedisManager.GetClient)
        //    {
        //        try
        //        {
        //            IDictionary<string, T> list = client.GetAll<T>(key);
        //            return list;
        //        }
        //        catch (Exception ex)
        //        {
        //           Log.WriteLog("GetList: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
        //            return null;
        //        }
        //    }
        //}
        public List<T> GetList(string key)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    List<T> list = client.Get<List<T>>(key);
                    return list;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("GetList: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    return null;
                }
            }

        }

        //public IDictionary<string, string> GetStringList(string[] key)
        //{
        //    using (var client = RedisManager.GetClient)
        //    {
        //        try
        //        {
        //            IDictionary<string, string> list = client.GetAll<string>(key);
        //            return list;
        //        }
        //        catch (Exception ex)
        //        {
        //           Log.WriteLog("GetStringList: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
        //            return null;
        //        }
        //    }
        //}

        public bool Remove(string key)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    bool res = client.Remove(key);
                    return res;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("Remove: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                    return false;
                }
            }
        }

        public void RemoveAll(string[] keys)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    client.RemoveAll(keys);
                }
                catch (Exception ex)
                {
                   Log.WriteLog("RemoveAll: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                }
            }
        }
        /// <summary>
        /// 删除某个前缀的缓存 一般为表名 TB_Account_
        /// </summary>
        /// <param name="keypat"></param>
        public void RemoveTable(string keypat)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    List<string> keys = client.SearchKeys(keypat + "*");
                    client.RemoveAll(keys);
                }
                catch (Exception ex)
                {
                   Log.WriteLog("RemoveAll: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                }
            }
        }

        public bool SetEntryInHash(string hashId, string key, string value)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    bool res = client.SetEntryInHash(hashId, key, value);
                    return res;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("SetEntryInHash: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                }
                return false;
            }
        }


        public List<string> GetHashKeys(string hashId)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    List<string> res = client.GetHashKeys(hashId);
                    return res;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("GetHashKeys: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                }
                return null;
            }
        }

        public List<string> GetHashValues(string key)
        {
            using (var client = RedisManager.GetClient)
            {

                try
                {
                    List<string> res = client.GetHashValues(key);
                    return res;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("GetHashKeys: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                }
                return null;
            }
        }

        public bool RemoveEntryFromHash(string hashId, string key)
        {
            using (var client = RedisManager.GetClient)
            {
                try
                {
                    bool res = client.RemoveEntryFromHash(hashId, key);
                    return res;
                }
                catch (Exception ex)
                {
                   Log.WriteLog("GetHashKeys: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Redis);
                }
                return false;
            }
        }

        /// <summary>
        /// 清除当前数据库
        /// </summary>
        public void FlushDB()
        {
            using (var client = RedisManager.GetClient)
            {
                client.FlushDb();
            }
        }
        /// <summary>
        /// 清除整个Redis
        /// </summary>
        public void FlushAll()
        {
            using (var client = RedisManager.GetClient)
            {
                client.FlushAll();
            }
        }
    }

    //public static class Logger
    //{
    //    /// <summary>
    //    /// 写日志
    //    /// </summary>
    //    /// <param name="LogStr">日志内容</param>
    //    public static void WriteLog(string logStr)
    //    {
    //        try
    //        {
    //            logStr = DateTime.Now.ToString("HH:mm:ss:fff") + "　" + logStr;
    //            string strPath = AppDomain.CurrentDomain.BaseDirectory + "log"; //HttpRuntime.AppDomainAppPath+"log";
    //            strPath = strPath + "\\Redis";
    //            if (!System.IO.Directory.Exists(strPath))
    //                System.IO.Directory.CreateDirectory(strPath);
    //            strPath = strPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

    //            using (StreamWriter sw = new StreamWriter(strPath, true, System.Text.Encoding.Default))
    //            {
    //                sw.WriteLine(logStr);
    //            }
    //        }
    //        catch
    //        {
    //        }
    //    }
    //}

}
