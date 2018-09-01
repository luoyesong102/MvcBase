using log4net.Config;
using log4net.Core;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Esmart.Framework.Logging
{
    public class Log4netLogger : ILogger
    {
        #region level consts

        public enum LOG_TYPE
        {
            /// <summary>
            /// 跟踪日志，主要用来在调试使用（可用来跟踪）
            /// </summary>
            Trace = LOG_LEVEL_02Trace,
            /// <summary>
            /// 跟踪日志，主要用来调试使用
            /// </summary>
            Debug = LOG_LEVEL_03Debug,
            /// <summary>
            /// 普通业务日志
            /// </summary>
            Info = LOG_LEVEL_04Info,
            /// <summary>
            /// 重要业务日志（通知日志）
            /// </summary>
            Notice = LOG_LEVEL_05Notice,
            /// <summary>
            /// 业务日志错误（警告日志）
            /// </summary>
            Warn = LOG_LEVEL_06Warn,
            /// <summary>
            /// 记录调用外部服务日志（服务日志）
            /// </summary>
            Server = LOG_LEVEL_08Severe
        };

        internal const int LOG_LEVEL_01Verbose = 1;
        internal const int LOG_LEVEL_02Trace = 2;
        internal const int LOG_LEVEL_03Debug = 3;
        internal const int LOG_LEVEL_04Info = 4;
        internal const int LOG_LEVEL_05Notice = 5;
        internal const int LOG_LEVEL_06Warn = 6;
        internal const int LOG_LEVEL_07Error = 7;
        internal const int LOG_LEVEL_08Severe = 8;
        internal const int LOG_LEVEL_11Fatal = 11;

        #endregion

        #region singleton

        private static IZLog zlog = null;
        private static IZLog Instance
        {
            get
            {
                if (zlog == null)
                {
                    XmlConfigurator.Configure();
                    zlog = ZLogManager.GetLogger(typeof(Log4netLogger));
                }
                return zlog;
            }
        }
        #endregion


        public void Info(string key, string msg)
        {
            Instance.Log(key, msg, Convert.ToInt32(LOG_TYPE.Info));
        }

        public void Warnning(string key, string msg)
        {
            Instance.Log(key, msg, Convert.ToInt32(LOG_TYPE.Warn));
        }

        public void Error(string key, Exception exception)
        {
            Instance.Error(key, exception);
        }
        public void Trace(string key, string msg)
        {
            Instance.Trace(key, msg);
        }
    }

    interface IZLog : ILoggerWrapper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        void Log(string message, int level);
        void Log(string title, string message, int level);
        void Error(Exception ex);
        void Error(string title, Exception ex);

        void Trace(string title, string message);
        //void Error(string title, Exception ex, int level);
        //bool Is01VerboseEnabled { get; }
        //bool Is02TraceEnabled { get; }
        //bool Is03DebugEnabled { get; }
        //bool Is04InfoEnabled { get; }
        //bool Is05NoticeEnabled { get; }
        //bool Is06WarnEnabled { get; }
        //bool Is07ErrorEnabled { get; }
        //bool Is08SevereEnabled { get; }
        //bool Is11FatalEnabled { get; }
    }

    #region 重写的辅助类：ZLogger，类似Log4net中LogImpl的功能

    public class ZLogger : LoggerWrapperImpl, IZLog
    {
        #region 日志处理级别

        private Level m_level01Verbose;
        private Level m_level02Trace;
        private Level m_level03Debug;
        private Level m_level04Info;
        private Level m_level05Notice;
        private Level m_level06Warn;
        private Level m_level07Error;
        private Level m_level08Severe;
        private Level m_level11Fatal;

        //定义日志记录标准化的格式：
        // 0 - 服务器时间
        // 1 - 日志摘要
        // 2 - 服务器IP
        // 3 - 客户端IP
        // 4 - 当前用户ptNumId
        // 5 - 日志内容
        private static string CR = System.Environment.NewLine;

        private static string LOG_TMPL_01Verbose = @"<Verbose Titile={1} ServerTime={0} ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Verbose>";
        private static string LOG_TMPL_02Trace = @"<Trace Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Trace>";
        private static string LOG_TMPL_03Debug = @"<Debug  Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Debug>";
        private static string LOG_TMPL_04Info = @"<Info  Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Info>";
        private static string LOG_TMPL_05Notice = @"<Notice Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Notice>";
        private static string LOG_TMPL_06Warn = @"<Warn Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Warn>";
        private static string LOG_TMPL_07Error = @"<Error Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Error>";
        private static string LOG_TMPL_08Severe = @"<Severe Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Severe>";
        private static string LOG_TMPL_11Fatal = @"<Fatal Titile={1} ServerTime={0}  ServerIP={2} ClientIP={3} UserID={4}><Desc>{5}</Desc></Fatal>";

        #endregion

        #region 构造和辅助方法
        private readonly static Type ThisDeclaringType = typeof(ZLogger);
        private void LoggerRepositoryConfigurationChanged(object sender, EventArgs e)
        {
            ILoggerRepository repository = sender as ILoggerRepository;
            if (repository != null)
            {
                ReloadLevels(repository);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public ZLogger(global::log4net.Core.ILogger log)
            : base(log)
        {
            log.Repository.ConfigurationChanged += new LoggerRepositoryConfigurationChangedEventHandler(LoggerRepositoryConfigurationChanged);

            ReloadLevels(log.Repository);
        }

        #endregion

        #region 处理日志级别

        protected virtual void ReloadLevels(ILoggerRepository repository)
        {
            LevelMap levelMap = repository.LevelMap;

            m_level01Verbose = levelMap.LookupWithDefault(Level.Verbose);
            m_level02Trace = levelMap.LookupWithDefault(Level.Trace);
            m_level03Debug = levelMap.LookupWithDefault(Level.Debug);
            m_level04Info = levelMap.LookupWithDefault(Level.Info);
            m_level05Notice = levelMap.LookupWithDefault(Level.Notice);
            m_level06Warn = levelMap.LookupWithDefault(Level.Warn);
            m_level07Error = levelMap.LookupWithDefault(Level.Error);
            m_level08Severe = levelMap.LookupWithDefault(Level.Severe);
            m_level11Fatal = levelMap.LookupWithDefault(Level.Fatal);
        }

        #endregion

        #region IZLog 成员

        public void Log(string message, int level)
        {
            Log("[TitleMissing]", message, level);
        }

        public void Log(string title, string message, int level)
        {
            Level lvl = m_level04Info;

            #region removed

            switch (level)
            {
                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_01Verbose:
                    lvl = m_level01Verbose;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_02Trace:
                    lvl = m_level02Trace;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_03Debug:
                    lvl = m_level03Debug;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_04Info:
                    lvl = m_level04Info;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_05Notice:
                    lvl = m_level05Notice;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_06Warn:
                    lvl = m_level06Warn;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_07Error:
                    lvl = m_level07Error;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_08Severe:
                    lvl = m_level08Severe;
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_11Fatal:
                    lvl = m_level11Fatal;
                    break;
            }

            #endregion

            Logger.Log(ThisDeclaringType, lvl, GetLogMessage(title, message, level), null);
        }

        public void Error(Exception ex)
        {
            Error(ex.GetType().Name, ex);
        }

        public void Error(string title, Exception ex)
        {
            Error(title, ex, 7);
        }

        public void Trace(string title, string msg)
        {
            LogToOracleTrace(title, msg);
        }
        private void LogToOracleTrace(string title, string msg)
        {
            //Log_Error entity = new Log_Error();
            //entity.GUID = System.Guid.NewGuid().ToString();
            //entity.ClientIP = System.Web.HttpContext.Current.Request.UserHostAddress;
            //DateTime dtNow = DateTime.Now;
            //entity.CreateDay = Convert.ToDateTime(dtNow.ToShortDateString());
            //entity.Message = ex.Message;
            //entity.ServerIP = (System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0]).ToString();
            //entity.ServerTime = dtNow;
            //entity.Source = ex.Source;
            //entity.ExceptionToString = ex.ToString(); ;
            //entity.SysType = "CMS";
            //entity.TargetSite = ex.TargetSite==null ? "" : ex.TargetSite.ToString();
            //entity.Title = title;
            //entity.TypeName = ex.GetType().Name;
            //entity.Remark1 = "";
            //entity.Remark2 = "";
            //string userID;
            //if (System.Web.HttpContext.Current.Session["USER_INFO_KEY"] == null)
            //{
            //    userID = System.Web.HttpContext.Current.User.Identity.Name;
            //}
            //else
            //{
            //    userID = System.Web.HttpContext.Current.Session["USER_INFO_KEY"].ToString();
            //}
            //entity.UserID = userID;
            //LogToOracle.Insert(entity);

            //插入数据库
        }
        private void LogToOracle1(string title, Exception ex)
        {
            //Log_Error entity = new Log_Error();
            //entity.GUID = System.Guid.NewGuid().ToString();
            //entity.ClientIP = System.Web.HttpContext.Current.Request.UserHostAddress;
            //DateTime dtNow = DateTime.Now;
            //entity.CreateDay = Convert.ToDateTime(dtNow.ToShortDateString());
            //entity.Message = ex.Message;
            //entity.ServerIP = (System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0]).ToString();
            //entity.ServerTime = dtNow;
            //entity.Source = ex.Source;
            //entity.ExceptionToString = ex.ToString(); ;
            //entity.SysType = "CMS";
            //entity.TargetSite = ex.TargetSite==null ? "" : ex.TargetSite.ToString();
            //entity.Title = title;
            //entity.TypeName = ex.GetType().Name;
            //entity.Remark1 = "";
            //entity.Remark2 = "";
            //string userID;
            //if (System.Web.HttpContext.Current.Session["USER_INFO_KEY"] == null)
            //{
            //    userID = System.Web.HttpContext.Current.User.Identity.Name;
            //}
            //else
            //{
            //    userID = System.Web.HttpContext.Current.Session["USER_INFO_KEY"].ToString();
            //}
            //entity.UserID = userID;
            //LogToOracle.Insert(entity);

            //插入数据库
        }



        public void Error(string title, Exception ex, int level)
        {

            if (level >= Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_07Error)
            {
                Log(title, ExceptionFormatter.FormatException(ex), level);

                try
                {
                    this.LogToOracle1(title, ex);
                }
                catch
                {

                }
            }
            else
            {
                Exception exgen = new Exception("日志记录失败：尝试记录未达级别的异常信息！");
                Error("日志记录失败", exgen);
            }
        }


        #region formater

        private string GetLogMessage(string title, string message, int level)
        {
            string[] rts = new string[6];

            rts[0] = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
            rts[1] = title.Replace("/", "-");
            rts[2] = (System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0]).ToString();
            System.Net.IPHostEntry ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

            if (ips != null && ips.AddressList.Length > 0)
            {
                rts[3] = ips.AddressList[0].ToString();
            }
            else
            {
                rts[3] = "";
            }
            rts[4] = "UserName";
            rts[5] = message;

            string rt = string.Empty;

            switch (level)
            {

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_01Verbose:
                    rt = string.Format(LOG_TMPL_01Verbose, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_02Trace:
                    rt = string.Format(LOG_TMPL_02Trace, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_03Debug:
                    rt = string.Format(LOG_TMPL_03Debug, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_04Info:
                    rt = string.Format(LOG_TMPL_04Info, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_05Notice:
                    rt = string.Format(LOG_TMPL_05Notice, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_06Warn:
                    rt = string.Format(LOG_TMPL_06Warn, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_07Error:
                    rt = string.Format(LOG_TMPL_07Error, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_08Severe:
                    rt = string.Format(LOG_TMPL_08Severe, rts);
                    break;

                case Esmart.Framework.Logging.Log4netLogger.LOG_LEVEL_11Fatal:
                    rt = string.Format(LOG_TMPL_11Fatal, rts);
                    break;
            }

            return rt;
        }

        #endregion

        public bool Is01VerboseEnabled
        {
            get { return Logger.IsEnabledFor(m_level01Verbose); }
        }

        public bool Is02TraceEnabled
        {
            get { return Logger.IsEnabledFor(m_level02Trace); }
        }

        public bool Is03DebugEnabled
        {
            get { return Logger.IsEnabledFor(m_level03Debug); }
        }

        public bool Is04InfoEnabled
        {
            get { return Logger.IsEnabledFor(m_level04Info); }
        }

        public bool Is05NoticeEnabled
        {
            get { return Logger.IsEnabledFor(m_level05Notice); }
        }

        public bool Is06WarnEnabled
        {
            get { return Logger.IsEnabledFor(m_level06Warn); }
        }

        public bool Is07ErrorEnabled
        {
            get { return Logger.IsEnabledFor(m_level07Error); }
        }

        public bool Is08SevereEnabled
        {
            get { return Logger.IsEnabledFor(m_level08Severe); }
        }

        public bool Is11FatalEnabled
        {
            get { return Logger.IsEnabledFor(m_level11Fatal); }
        }

        #endregion
    }

    #endregion

    #region 重写的辅助类：ZLogManager，类似Log4net中LogManager的功能

    sealed class ZLogManager
    {
        public static IZLog Exists(string name)
        {
            return Exists(Assembly.GetCallingAssembly(), name);
        }
        public static IZLog Exists(string repository, string name)
        {
            return WrapLogger(LoggerManager.Exists(repository, name));
        }
        public static IZLog Exists(Assembly repositoryAssembly, string name)
        {
            return WrapLogger(LoggerManager.Exists(repositoryAssembly, name));
        }
        public static IZLog[] GetCurrentLoggers()
        {
            return GetCurrentLoggers(Assembly.GetCallingAssembly());
        }
        public static IZLog[] GetCurrentLoggers(string repository)
        {
            return WrapLoggers(LoggerManager.GetCurrentLoggers(repository));
        }
        public static IZLog[] GetCurrentLoggers(Assembly repositoryAssembly)
        {
            return WrapLoggers(LoggerManager.GetCurrentLoggers(repositoryAssembly));
        }
        public static IZLog GetLogger(string name)
        {
            return GetLogger(Assembly.GetCallingAssembly(), name);
        }
        public static IZLog GetLogger(string repository, string name)
        {
            return WrapLogger(LoggerManager.GetLogger(repository, name));
        }
        public static IZLog GetLogger(Assembly repositoryAssembly, string name)
        {
            return WrapLogger(LoggerManager.GetLogger(repositoryAssembly, name));
        }
        public static IZLog GetLogger(Type type)
        {
            return GetLogger(Assembly.GetCallingAssembly(), type.FullName);
        }
        public static IZLog GetLogger(string repository, Type type)
        {
            return WrapLogger(LoggerManager.GetLogger(repository, type));
        }
        public static IZLog GetLogger(Assembly repositoryAssembly, Type type)
        {
            return WrapLogger(LoggerManager.GetLogger(repositoryAssembly, type));
        }
        public static void Shutdown()
        {
            LoggerManager.Shutdown();
        }
        public static void ShutdownRepository()
        {
            ShutdownRepository(Assembly.GetCallingAssembly());
        }
        public static void ShutdownRepository(string repository)
        {
            LoggerManager.ShutdownRepository(repository);
        }
        public static void ShutdownRepository(Assembly repositoryAssembly)
        {
            LoggerManager.ShutdownRepository(repositoryAssembly);
        }
        public static void ResetConfiguration()
        {
            ResetConfiguration(Assembly.GetCallingAssembly());
        }
        public static void ResetConfiguration(string repository)
        {
            LoggerManager.ResetConfiguration(repository);
        }
        public static void ResetConfiguration(Assembly repositoryAssembly)
        {
            LoggerManager.ResetConfiguration(repositoryAssembly);
        }
        [Obsolete("Use GetRepository instead of GetLoggerRepository")]
        public static ILoggerRepository GetLoggerRepository()
        {
            return GetRepository(Assembly.GetCallingAssembly());
        }
        [Obsolete("Use GetRepository instead of GetLoggerRepository")]
        public static ILoggerRepository GetLoggerRepository(string repository)
        {
            return GetRepository(repository);
        }
        [Obsolete("Use GetRepository instead of GetLoggerRepository")]
        public static ILoggerRepository GetLoggerRepository(Assembly repositoryAssembly)
        {
            return GetRepository(repositoryAssembly);
        }
        public static ILoggerRepository GetRepository()
        {
            return GetRepository(Assembly.GetCallingAssembly());
        }
        public static ILoggerRepository GetRepository(string repository)
        {
            return LoggerManager.GetRepository(repository);
        }
        public static ILoggerRepository GetRepository(Assembly repositoryAssembly)
        {
            return LoggerManager.GetRepository(repositoryAssembly);
        }

        [Obsolete("Use CreateRepository instead of CreateDomain")]
        public static ILoggerRepository CreateDomain(Type repositoryType)
        {
            return CreateRepository(Assembly.GetCallingAssembly(), repositoryType);
        }
        public static ILoggerRepository CreateRepository(Type repositoryType)
        {
            return CreateRepository(Assembly.GetCallingAssembly(), repositoryType);
        }

        [Obsolete("Use CreateRepository instead of CreateDomain")]
        public static ILoggerRepository CreateDomain(string repository)
        {
            return LoggerManager.CreateRepository(repository);
        }
        public static ILoggerRepository CreateRepository(string repository)
        {
            return LoggerManager.CreateRepository(repository);
        }

        [Obsolete("Use CreateRepository instead of CreateDomain")]
        public static ILoggerRepository CreateDomain(string repository, Type repositoryType)
        {
            return LoggerManager.CreateRepository(repository, repositoryType);
        }
        public static ILoggerRepository CreateRepository(string repository, Type repositoryType)
        {
            return LoggerManager.CreateRepository(repository, repositoryType);
        }

        [Obsolete("Use CreateRepository instead of CreateDomain")]
        public static ILoggerRepository CreateDomain(Assembly repositoryAssembly, Type repositoryType)
        {
            return LoggerManager.CreateRepository(repositoryAssembly, repositoryType);
        }
        public static ILoggerRepository CreateRepository(Assembly repositoryAssembly, Type repositoryType)
        {
            return LoggerManager.CreateRepository(repositoryAssembly, repositoryType);
        }
        public static ILoggerRepository[] GetAllRepositories()
        {
            return LoggerManager.GetAllRepositories();
        }
        private static IZLog WrapLogger(global::log4net.Core.ILogger logger)
        {
            return (IZLog)s_wrapperMap.GetWrapper(logger);
        }
        private static IZLog[] WrapLoggers(global::log4net.Core.ILogger[] loggers)
        {
            IZLog[] results = new IZLog[loggers.Length];
            for (int i = 0; i < loggers.Length; i++)
            {
                results[i] = WrapLogger(loggers[i]);
            }
            return results;
        }
        private static ILoggerWrapper WrapperCreationHandler(global::log4net.Core.ILogger logger)
        {
            return new ZLogger(logger);
        }
        private static readonly WrapperMap s_wrapperMap = new WrapperMap(new WrapperCreationHandler(WrapperCreationHandler));
    }

    #endregion


    #region 日志异常格式化 ：ExceptionFormatter
    public static class ExceptionFormatter
    {        /// <summary>
        /// 得到指定名称和值的赋值字符串
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <author>龚正</author>
        public static string GetNameValue(string Name, object Value)
        {
            if (Value != null)
                return "<" + Name + ">" + Value.ToString() + "</" + Name + ">\r\n";
            else
                return "<" + Name + "/>\r\n";
        }
        public static string FormatException(Exception e)
        {
            StringBuilder sbXml = new StringBuilder();
            sbXml.AppendLine();
            sbXml.AppendLine(GetNameValue("TypeName", e.GetType().Name));
            sbXml.AppendLine(GetNameValue("Message", e.Message));
            sbXml.AppendLine(GetNameValue("StackTrace", e.StackTrace));
            sbXml.AppendLine(GetNameValue("Source", e.Source));
            sbXml.AppendLine(GetNameValue("TargetSite", e.TargetSite));

            if (e.InnerException != null)
            {
                sbXml.AppendLine("<InnerException>\r\n");
                sbXml.AppendLine(FormatException(e.InnerException));
                sbXml.AppendLine("</InnerException>\r\n");
            }

            return sbXml.ToString();
        }
    }
    #endregion
}
