/*
 ===============================================================================
 * 作者        ：chalei.wu
 * 编写时间    :2014-11-23
 * 修改历史记录：
 * 存在的bug   ：
 * 待优化方案  ：
 ===============================================================================
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmart.Framework.Model;

namespace Esmart.Framework.Logging
{
   /// <summary>
   /// 日记记录
   /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 写信息日志
        /// </summary>
        void Info(string key, string msg);


        /// <summary>
        /// 写警告日志
        /// </summary>
        void Warnning(string key, string msg);
        void Trace(string key, string msg);
        /// <summary>
        /// 写错误日志
        /// </summary>
        void Error(string key, Exception exception);

    }
}
