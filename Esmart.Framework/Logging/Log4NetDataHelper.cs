using System;
using System.IO;
using System.Text;
using log4net;
using Newtonsoft.Json;
using Esmart.Framework.Config;
using Esmart.Framework;
using Esmart.Framework.Exceptions;
using Esmart.Framework.Model;

namespace Esmart.Framework.Logging
{
    public static class Log4NetDataHelper
    {
        static Log4NetDataHelper()
        {
            //初始化log4net配置
            var config = CachedConfigContext.Current.ConfigService.GetConfig("log4net");
              //重写log4net配置里的连接字符串
              config = config.Replace("{connectionString}", GlobalConfig.LogHelpDB);
            var ms = new MemoryStream(Encoding.Default.GetBytes(config));
            log4net.Config.XmlConfigurator.Configure(ms);
          
        }

        public static void Debug(LoggerType loggerType, object message, Exception e)
        {
            
                var logger = log4net.LogManager.GetLogger(loggerType.ToString());
            logger.Debug(SerializeObject(message), e);
            
        }

        public static void Error(LoggerType loggerType, object message, Exception e)
        {
          
                var logger = log4net.LogManager.GetLogger(loggerType.ToString());
                logger.Error(SerializeObject(message), e);
         
        }

        public static void Info(LoggerType loggerType, object message, Exception e)
        {
           
                var logger = log4net.LogManager.GetLogger(loggerType.ToString());
                logger.Info(SerializeObject(message), e);
         
        }

        public static void Fatal(LoggerType loggerType, object message, Exception e)
        {
           
                var logger = log4net.LogManager.GetLogger(loggerType.ToString());
                logger.Fatal(SerializeObject(message), e);
          
        }

        public static void Warn(LoggerType loggerType, object message, Exception e)
        {
            
                var logger = log4net.LogManager.GetLogger(loggerType.ToString());
                logger.Warn(SerializeObject(message), e);
          
        }

        private static object SerializeObject(object message)
        {
            if (message is string || message == null)
                return message;
            else
                return JsonConvert.SerializeObject(message, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }

  
}
