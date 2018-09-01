using System;
using Esmart.Framework.Utilities;

namespace Esmart.Framework.Cache
{
    public abstract class ServiceFactory
    {
        public abstract T CreateService<T>() where T : class;
    }

    /// <summary>
    /// 直接引用提供服务
    /// </summary>
    public class RefServiceFactory : ServiceFactory
    {
        public override T CreateService<T>()
        {
            var interfaceName = typeof(T).Name;
           //第一次通过反射创建服务实例，然后缓存住
            return CacheHelper.Get<T>(string.Format("Service_{0}", interfaceName), () =>
            {
                return AssemblyHelper.FindTypeByInterface<T>();
            });
           
           
               
        }
    }

    /// <summary>
    /// 通过Wcf提供服务
    /// </summary>
    public class WcfServiceFactory : ServiceFactory
    {
        public override T CreateService<T>()
        {
            //TODO
            //需实现WCF Uri来自配置文件
            var uri = string.Empty;
            var proxy = WcfServiceProxy.CreateServiceProxy<T>(uri);
            return proxy;
        }
    }
}
