using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esmart.Framework.Redis
{
    public enum LogType
    {
        Mongodb = 1,
        Redis = 2,
        Track = 3,
        Queue = 4,
        IPCheck = 5,
        ES = 6,
        JS = 7,
        Other = 0
    }

    public class Log
    {
        #region 写日志

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="LogStr">日志内容</param>
        /// <param name="i">枚举LogType</param>
        public static void WriteLog(string LogStr, int i)
        {
            try
            {
                //string strPath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();
                string strPath = AppDomain.CurrentDomain.BaseDirectory + "\\log";

                if (!System.IO.Directory.Exists(strPath))
                    System.IO.Directory.CreateDirectory(strPath);

                switch (i)
                {
                    case 1:
                        strPath = strPath + "\\mongodb_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    case 2:
                        strPath = strPath + "\\redis_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    case 3:
                        strPath = strPath + "\\track_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    case 4:
                        strPath = strPath + "\\queue_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    case 5:
                        strPath = strPath + "\\ipcheck_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    case 6:
                        strPath = strPath + "\\es_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    case 7:
                        strPath = strPath + "\\js_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                    default:
                        strPath = strPath + "\\other_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        break;
                }

                using (StreamWriter sw = new StreamWriter(strPath, true, System.Text.Encoding.Default))
                {
                    sw.WriteLine("\r\n----------" + DateTime.Now.ToString("HH:mm:ss:fff") + "----------\r\n" + LogStr);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                WriteLogToOther(LogStr + "\r\n" + ex.Message.ToString() + " Source:" + ex.Source + " TargetSite:" + ex.TargetSite.ToString() + " InnerException:" + Convert.ToString(ex.InnerException));
            }
        }

        public static void WriteLogToOther(string LogStr, bool isOrginal = false)
        {
            try
            {
                string txtDpath = AppDomain.CurrentDomain.BaseDirectory + "\\log";
                if (!System.IO.Directory.Exists(txtDpath))
                    System.IO.Directory.CreateDirectory(txtDpath);
                string txtFullpath = txtDpath + "\\local_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                using (StreamWriter sw = new StreamWriter(txtFullpath, true, System.Text.Encoding.Default))
                {
                    if (isOrginal)
                        sw.WriteLine(LogStr);
                    else
                        sw.WriteLine("\r\n----------" + DateTime.Now.ToString("HH:mm:ss:fff") + "----------\r\n" + LogStr + "\r\n");
                    sw.Flush();
                    sw.Close();
                }
            }
            catch { }
        }


        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="LogStr">日志内容</param>
        public static void WriteLog(string LogStr)
        {
            WriteLog(LogStr, "", "");
        }
        public static void WriteLog(string LogStr, string fname, string Dname)
        {
            MulitWriteLog(LogStr, fname, Dname, false);
            MulitWriteLog("", fname, Dname, true);
        }

        static Dictionary<string, StreamWriter> logDict = new Dictionary<string, StreamWriter>();
        public static void MulitWriteLog(string LogStr, string fname, string Dname, bool isAll)
        {
            //StreamWriter sw = null;
            try
            {
                string folderPath = AppDomain.CurrentDomain.BaseDirectory + "log" + (string.IsNullOrEmpty(Dname) ? "" : "\\" + Dname);//HttpRuntime.AppDomainAppPath+"log";
                string fullPath = string.Empty;
                if (string.IsNullOrEmpty(fname))
                {
                    fullPath = folderPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                }
                else
                {
                    folderPath = folderPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
                    fullPath = folderPath + "\\" + fname + ".txt";
                }
                if (!isAll)
                {
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    if (!logDict.ContainsKey(fullPath))
                        logDict.Add(fullPath, new StreamWriter(fullPath, true, Encoding.UTF8));

                    logDict[fullPath].WriteLine("\r\n----------" + DateTime.Now.ToString("hh:mm:ss:fff") + "----------\r\n" + LogStr);
                }
                else
                {
                    if (logDict.ContainsKey(fullPath) && logDict[fullPath] != null)
                    {
                        logDict[fullPath].Flush();
                        logDict[fullPath].Close();
                        logDict[fullPath].Dispose();
                        logDict.Remove(fullPath);
                    }
                }
            }
            catch { }
        }
        #endregion

    }
}
