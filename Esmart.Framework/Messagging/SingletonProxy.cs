using System;
using System.Reflection;
using System.Threading;
using Esmart.Framework.Exceptions;
namespace Esmart.Framework.Messagging
{
    /// <summary>
    /// 单例对象创建代理类
    /// </summary>
    /// <typeparam name="T">单例对象的类型</typeparam>
    public class SingletonProxy<T> where T : class
    {
        private static T _instance = default(T);

        /// <summary>
        /// 创建单丽对象
        /// </summary>
        /// <param name="instance">创建对象的委托</param>
        /// <returns>类的单例对象</returns>
        public static T Create(Func<T> instance)
        {
            if (_instance == null)
            {
                lock (typeof(SingletonProxy<T>))
                {
                    if (_instance == null)
                    {
                        lock (typeof(SingletonProxy<T>))
                        {
                            if (typeof(T).GetConstructors(BindingFlags.Public).Length > 0)
                            {
                                throw new BusinessException(typeof(T) + " 不能有公开的构造函数");
                            }
                            var temp = instance();
                            Thread.MemoryBarrier();
                            _instance = temp;
                        }
                    }
                }
            }
            return _instance;
        }
    }
}
