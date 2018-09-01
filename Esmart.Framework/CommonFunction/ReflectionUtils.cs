using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;

namespace Esmart.Framework.Utilities
{
    /// <summary>
    /// 从当前应用程序域中获取类型数据
    /// </summary>
    public class ReflectionUtils
    {
        /// <summary>
        /// 从当前应用程序域中的所有程序集中获取满足条件的类型
        /// </summary>
        /// <param name="where">筛选类型的表达式</param>
        /// <returns>满足条件的类型</returns>
        public static IEnumerable<Type> FindType(Func<Type, bool> where)
        {
           
            var assCollction = AppDomain.CurrentDomain.GetAssemblies();
            
            var results = Enumerable.Empty<Type>();
            foreach (var assembly in assCollction)
            {

                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (where(type))
                        {
                            results = results.Concat(new[] { type });
                        }
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    //Trace.WriteLog("类型载入发生错误", LogLevel.Warn, ex);
                     continue;
                    //results = results.Concat(ex.Types);
                }
                catch(Exception ex)
                {
                  
                    throw ex;
                }                
            }
            return results;           
        }

        /// <summary>
        /// 从当前应用程序域中的所有程序集中获取满足条件的类型并且该类被特定特性所标识
        /// </summary>
        /// <typeparam name="TAttr">类特性</typeparam>
        /// <param name="where">筛选类型的表达式</param>
        /// <param name="inherit">继承自指定特性的特性是否保留</param>
        /// <returns>满足条件的类型</returns>
        public static IEnumerable<Type> FindType<TAttr>(Func<TAttr,Type, bool> where,bool inherit = false) where TAttr:Attribute
        {
            return FindType(t => {
                if (t.IsDefined(typeof(TAttr), inherit))
                {
                    var attr = (TAttr)t.GetCustomAttributes(typeof(TAttr), inherit).FirstOrDefault();
                    return (attr != null && where(attr,t));                     
                }
                return false;
            });
        }

        /// <summary>
        /// 从当前应用程序域中的所有程序集中获取满足条件的方法并且该类和方法被特定特性所标识
        /// </summary>
        /// <typeparam name="CAtt">类特性</typeparam>
        /// <typeparam name="MAttr">方法特性</typeparam>
        /// <param name="classFilter">筛选类型的表达式</param>
        /// <param name="methodFilter">筛选方法的表达式</param>
        /// <returns>满足条件的方法</returns>
        public static MethodInfo GetMethod<CAtt, MAttr>(Func<CAtt, Type, bool> classFilter, Func<MAttr, bool> methodFilter)
            where CAtt : Attribute
            where MAttr : Attribute
        {
            var types = FindType<CAtt>((c, b) => classFilter(c, b));
            foreach (var type in types)
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attr in method.GetCustomAttributes(true))
                    {
                        if (attr is MAttr)
                        {
                            if (methodFilter((MAttr)attr))
                            {
                                return method;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
