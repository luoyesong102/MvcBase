using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using EasyNetQ.Loggers;
using EasyNetQ;
namespace Esmart.Framework.RabbitMq
{

  public  class LogFactory
    {
      public static string LogType { get { return ConfigurationManager.AppSettings["LogType"] + ""; } }

      public static IEasyNetQLogger GetLogType()
      {
          if (LogType == "ConsoleLogger")
          {
              return new ConsoleLogger();
          }
          else if (LogType == "DelegateLogger")
          {
              var delelog = new DelegateLogger();
              delelog.DebugWriteDelegate = (a, b) =>
              {
                  Log.WriteLog("Debug:"+a);
              };
              delelog.ErrorWriteDelegate = (a, b) =>
              {
                  Log.WriteLog("Error:"+a);
              };
              delelog.InfoWriteDelegate = (a, b) =>
              {
                  Log.WriteLog("Info:"+a);
              };
              return delelog;
          }
          else if (LogType == "NullLogger")
          {
              return new NullLogger();
          }
          else
          {
              return new NullLogger();
          }
      }
    }
}
