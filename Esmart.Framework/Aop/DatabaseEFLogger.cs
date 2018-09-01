using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esmart.Framework.Exceptions;
using Esmart.Framework.Logging;
using Esmart.Framework.Model;

namespace Esmart.Framework.Aop
{
    public class DatabaseEFLogger : IDbCommandInterceptor
    {

        static readonly ConcurrentDictionary<DbCommand, DateTime> MStartTime = new ConcurrentDictionary<DbCommand, DateTime>();
        //记录开始执行时的时间
        private static void OnStart(DbCommand command)
        {
            MStartTime.TryAdd(command, DateTime.Now);
        }
        private static void Log<T>(DbCommand command, DbCommandInterceptionContext<T> interceptionContext)
        {

            DateTime startTime;
            TimeSpan duration;
            //得到此command的开始时间
            MStartTime.TryRemove(command, out startTime);
            if (startTime != default(DateTime))
            {
                duration = DateTime.Now - startTime;
            }
            else
                duration = TimeSpan.Zero;

            var parameters = new StringBuilder();
            //循环获取执行语句的参数值
            foreach (DbParameter param in command.Parameters)
            {
                parameters.AppendLine(param.ParameterName + " " + param.DbType + " = " + param.Value);
            }

            //判断语句是否执行时间超过1秒或是否有错
            if (duration.TotalSeconds > 1 || interceptionContext.Exception != null)
            {
                //这里编写记录执行超长时间SQL语句和错误信息的代码
                string sql = command.CommandText;

                Esmart.Framework.Logging.LogManager.CreateLog4net().Error(ConstantDefine.Error, new Exception(sql+"|"+ parameters));
            }
            else
            {
               
                string sql = command.CommandText;
                Esmart.Framework.Logging.LogManager.CreateLog4net().Error(ConstantDefine.Error, new Exception(sql + "|" + parameters));
                //这里编写你自己记录普通SQL语句的代码
            }


        }
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            Log(command, interceptionContext);
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            OnStart(command);
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {

            Log(command, interceptionContext);
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            OnStart(command);
        }
        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            Log(command, interceptionContext);
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {

            OnStart(command);
        }
    }
}
