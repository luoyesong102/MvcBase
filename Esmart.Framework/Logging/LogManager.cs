
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmart.Framework.Model;
using System.Threading;

namespace Esmart.Framework.Logging
{
    public class LogManager
    {
      

      


        static LogManager()
        {
          
        }

        /// <summary>
        /// 创建Redis缓存
        /// </summary>
        /// <returns></returns>
        public static ILogger CreateTpoLog()
        {
            try
            {
                if (ConstantDefine.NotUserLog)
                {
                    return new NotImpLog();
                }
               
                return new TpoLogger();
            }
            catch (Exception ex)
            {
               
                ConstantDefine.SetNotUserLog();
                CreateLog4net().Error("rabbit错误", ex);
            }
            return new NotImpLog();
        }


       
        public static ILogger CreateLog4net()
        {
            if (!ConstantDefine.isprofile)
            {
                return new NotImpLog();
            }
            return new Log4netLogger();
        }


      
        ~LogManager()
        {
          
        }
    }
}
