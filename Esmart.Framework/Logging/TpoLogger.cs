using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Esmart.Framework.Caching;
using Esmart.Framework.Model;

namespace Esmart.Framework.Logging
{
    public class TpoLogger : ILogger
    {

        private static int appID = 0;
    

        internal TpoLogger()
        {
          
        }


        public void SendQueue(object obj)
        {

          
        }

        private static int GetAppID()
        {
            if (appID == 0)
            {
                appID = ConstantDefine.AppID;
            }
            return appID;
        }


        public void Info(string key, string msg )
        {
           

        }

        public void Warnning(string key, string msg)
        {
             
        }
        public void Trace(string key, string msg)
        {
           
        }
        public void Error(string key, Exception exception)
        {

            try
            {
                int count = 0;
                while (exception.InnerException != null && count < 5)
                {
                    exception = exception.InnerException;
                    count++;
                }

                LogException model = new LogException();

                model.CreateTime = DateTime.Now;
                model.IP = Esmart.Framework.Model.CommonFunction.GetClientUserIP();

                model.Title = key;
                if (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                if (!string.IsNullOrEmpty(exception.Message))
                {
                    model.Message = exception.Message.Length < 2500 ? exception.Message : exception.Message.Substring(0, 2500);
                }

                if (!string.IsNullOrEmpty(exception.Source))
                {
                    model.Source = exception.Source.Length < 2500 ? exception.Source : exception.Source.Substring(0, 2500);
                }

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    model.StackTrace = exception.StackTrace.Length < 2500 ? exception.StackTrace : exception.StackTrace.Substring(0, 2500);
                }

                Esmart.Framework.DB.SQLManager.CreateSqlEngine("LogHelpDB").InsertObjectAsy(model);
            }
            catch (Exception ex)
            {

            }

        }


        private string GetUserIP()
        {
            //System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
            return System.Net.Dns.GetHostName();
        }
    }

    class Log
    {
        public string LogType { get; set; }

        public LogException Exception { get; set; }
    }
}
