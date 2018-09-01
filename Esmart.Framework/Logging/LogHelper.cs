 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Esmart.Framework.Model;
namespace Esmart.Framework.Logging
{

    public class LogHelper
    {
        static object _lock = new object();
        private static Thread ExceptionLogThread;
        private static bool IsExceptionThreadLive = true;
        private static Queue<LogRequestInfo> queue;

        public const string LogTrace = "trace";
        public const string LogError = "error";

        static LogHelper()
        {
            queue = new Queue<LogRequestInfo>();
            ExceptionLogThread = new Thread(SaveLogData);
            ExceptionLogThread.IsBackground = true;
            ExceptionLogThread.Start();


        }

        public static void StartExceptionLogThread()
        {
        }

        public static void EnQueueTrace(string msg,string msg2,string msg3,string type)
        {
            LogRequestInfo log = new LogRequestInfo();
            log.CreateDate = DateTime.Now;
            log.LogType = LogTrace;
            log.Type = type;
            log.Message = msg;
            log.Message2 = msg2;
            log.Message3 = msg3;
            lock (queue)
            {
                queue.Enqueue(log);
            }
        }
        public static void EnQueue(LogRequestInfo exception)
        {
            lock (queue)
            {
                queue.Enqueue(exception);
            }
        }
        public static LogRequestInfo DeQueue()
        {
            lock (queue)
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
                return null;
            }
        }
        public static List<LogRequestInfo> DeQueueList()
        {
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    List<LogRequestInfo> ret = queue.ToList();
                    queue.Clear();
                    return ret;
                }
                return null;
            }
        }

      

        static void SaveLogData()
        {
            while (IsExceptionThreadLive)
            {
                try
                {
                    List<LogRequestInfo> logs = DeQueueList();
                    if (logs != null)
                    {
                        if (!ConstantDefine.NotUserLog)
                        {
                            Esmart.Framework.DB.SQLManager.CreateSqlEngine("LogHelpDB").InsertListObject(logs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SaveFileLog("SaveLogData", ex); 
                }
                Thread.Sleep(1000);
            }
        }

        public static void SaveFileLog(object msg, Exception ex)
        {
            log4net.LogManager.GetLogger(typeof(LogHelper)).Error(msg, ex);
        }
    }
}