using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using Esmart.Framework.Logging;
using Newtonsoft.Json;
using Esmart.Framework.Model;
using Esmart.Framework.Exceptions;

namespace Esmart.Framework.Aop
{
    /// <summary>
    /// http://www.cnblogs.com/zhengwl/p/5433181.html  autofac castle    [Intercept(typeof(CallLogger))]          [AopAttribute]   ContextBoundObject     
    /// </summary>
    public class AopAttribute : ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            AopProxy realProxy = new AopProxy(serverType);
            return realProxy.GetTransparentProxy() as MarshalByRefObject;
        }
    }


    public class AopProxy : RealProxy
    {

        /// <summary>
        /// 是否记录错误日志
        /// </summary>
        private static bool IsErrorLog
        {
            get
            {
                //var result = Convert.ToBoolean(ConfigurationManager.AppSettings["IsErrorLog"]);
                return true;
            }
        }
        /// <summary>
        /// 是否记录所有日志
        /// </summary>
        private static bool IsAllLog
        {
            get
            {
                return true;
               // return Convert.ToBoolean(ConfigurationManager.AppSettings["IsAllLog"]);
            }
        }
        /// <summary>
        /// 慢查询阀值
        /// </summary>
        private static int SlowQuery
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["SlowQuery"]);
            }
        }


        /// <summary>
        /// AOP协议用于记录运行日志
        /// </summary>
        /// <param name="serverType"></param>
        public AopProxy(Type serverType)
            : base(serverType) { }
        public override IMessage Invoke(IMessage msg)
        {
            Logger log = null;
            string returnvalue = null;//返回值
            bool IsErrorMsg = false;
            if (IsErrorLog || IsAllLog)
            {
                log = new Logger
                {
                    CreateDate = DateTime.Now,
                    Project = "SesameLingo",
                    LogType = (int)LogType.Warring,
                    Args = ""
                };
            }
            object returnIMessage = null;
            if (msg is IConstructionCallMessage)
            {
                //构造函数调用
                IConstructionCallMessage constructCallMsg = msg as IConstructionCallMessage;
                IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)msg);
                RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);
                // Console.WriteLine("Call constructor");
                object[] args = constructCallMsg.Args;
                for (int i = 0; i < args.Length; i++)
                {
                    log.Args += (args[i] != null ? args[i].ToString() : "") + ",";
                }
                returnIMessage = constructionReturnMessage;
            }
            else
            {
                //非构造函数调用

                IMethodCallMessage callMsg = msg as IMethodCallMessage;
                IMessage message;
                try
                {
                    log.MethodName = callMsg.MethodName;
                    log.TypeName = callMsg.TypeName;

                    object[] args = callMsg.Args;
                    object o = callMsg.MethodBase.Invoke(GetUnwrappedServer(), args);
                    message = new ReturnMessage(o, args, args.Length, callMsg.LogicalCallContext, callMsg);
                    #region 记录方法输入参数
                    if (args != null && args.Length > 0)
                        for (int i = 0; i < args.Length; i++)
                        {
                            log.Args += (args[i] != null ? args[i].ToString() : "") + ",";
                        }
                    #endregion

                }
                catch (Exception ex)
                {
                    message = new ReturnMessage(ex, callMsg);
                    IsErrorMsg = true;
                    if (IsErrorLog || IsAllLog)
                    {
                        log.ErrorMsg = Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                        log.LogType = (int)LogType.Error;
                    }
                }
                if (message.Properties["__Return"] != null)
                {
                    returnvalue = message.Properties["__Return"].ToString();
                }
                log.ReturnValue = returnvalue;
                //Console.WriteLine(returnvalue);
                returnIMessage = message;
            }


            if (IsAllLog)
            {
                log.EndDate = DateTime.Now;
                TimeSpan span = (TimeSpan)(log.EndDate - log.CreateDate);
                log.UseTime = span.Milliseconds;
                if (span.Milliseconds > SlowQuery)
                {
                    Task task = new Task(new Action(() =>
                    {
                        SystemErrorLogOper.Create(log);
                    }));
                    task.Start();
                }
            }
            else if (IsErrorLog && IsErrorMsg)
            {
                log.EndDate = DateTime.Now;
                TimeSpan span = (TimeSpan)(log.EndDate - log.CreateDate);
                log.UseTime = span.Milliseconds;
                Task task = new Task(new Action(() =>
                {
                    SystemErrorLogOper.Create(log);
                }));
                task.Start();
            }

            return returnIMessage as IMessage;
        }
    }


    public sealed class SystemErrorLogOper
    {
        public static void Create(Logger model)
        {
          
            string waringjson = JsonConvert.SerializeObject(model).ToString();
            //DB层的拦截在此处理请求日志，WEBapi-info,异常error ,业务异常
              LogHelper.EnQueueTrace("", waringjson, "", "fun response");

        }
    }

    //public enum LogType
    //{
    //    错误 = 1,
    //    一般信息 = 0
    //}

}
