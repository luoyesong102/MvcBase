using System;
using System.Linq;
using System.Reflection;

namespace Esmart.Framework.Utilities
{
    public abstract class SingleInstance<T>
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (ctors.Count() != 1)
            {
                throw new InvalidOperationException($"类型 {typeof(T)} 至少有一个构造函数！");
            }

            var ctor = ctors.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
            if (ctor == null)
            {
                throw new InvalidOperationException($"类型 {typeof(T)}的构造函数必须是私有的并且不带任何参数！");
            }

            return (T)ctor.Invoke(null);
        });

        public static T Instance => _instance.Value;
    }
}
