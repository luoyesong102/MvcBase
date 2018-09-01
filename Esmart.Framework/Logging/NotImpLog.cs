using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Logging
{
    public class NotImpLog:ILogger
    {
        public void Info(string key, string msg)
        {
            //未实现任何细节
        }

        public void Warnning(string key, string msg)
        {
            //未实现任何细节
        }

        public void Error(string key, Exception exception)
        {
            //未实现任何细节
        }

        public void Trace(string key, string msg)
        {
            //未实现任何细节
        }
    }
}
