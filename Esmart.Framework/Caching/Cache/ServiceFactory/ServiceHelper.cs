using System;
using System.Linq;
using System.Collections.Generic;
using Castle.DynamicProxy;

using Esmart.Framework.Exceptions;

namespace Esmart.Framework.Cache
{
    public partial class ServiceHelper
    {
        /// <summary>
        /// 暂时使用引用服务方式，可以改造成注入，或使用WCF服务方式
        /// </summary>
        public static ServiceFactory serviceFactory = new RefServiceFactory();
        
        /// <summary>
        /// 创建服务根据BLL接口
        /// </summary>
        public static T CreateService<T>() where T : class
        {
            var service = serviceFactory.CreateService<T>();

            //拦截，可以写日志....
            var generator = new ProxyGenerator();
            var dynamicProxy = generator.CreateInterfaceProxyWithTargetInterface<T>(
                service, new InvokeInterceptor());
            //var proxyGenerator = new ProxyGenerator();
           //var handler = new UserRoleInterceptorProxy[] { new UserRoleInterceptorProxy("zhz") };
           //IUserBLL iRole = proxyGenerator.CreateClassProxy<UserBLL>(handler);
           // iRole.GetUser("zzl");
            return dynamicProxy;
        }
    }
     

    internal class InvokeInterceptor : IInterceptor
    {
        public InvokeInterceptor()
        {   

        }
        
        /// <summary>
        /// 拦截方法
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();      
            }
            catch (Exception exception)
            {
                if (exception is BusinessException)
                    throw;
                
                var message = new
                {
                    exception = exception.Message,
                    exceptionContext = new
                    {
                        method = invocation.Method.ToString(),
                        arguments = invocation.Arguments,
                        returnValue = invocation.ReturnValue
                    }
                };

                //Log4NetHelper.Error(LoggerType.ServiceExceptionLog, message, exception);请求处理
                throw;
            }
        }
    }
}
