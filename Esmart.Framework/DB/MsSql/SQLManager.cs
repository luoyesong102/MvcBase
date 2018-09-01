using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.DB
{
    public class SQLManager
    {
        /// <summary>
        /// 创建引擎对象
        /// </summary>
        /// <param name="connectKey"></param>
        /// <returns></returns>
        public static IDbExec CreateSqlEngine(string connectKey)
        {
            TSqlExec exec = new TSqlExec() { ConnectKey = connectKey };
            return exec;
        }
    }
}
