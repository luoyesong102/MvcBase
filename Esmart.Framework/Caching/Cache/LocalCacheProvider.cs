using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Esmart.Framework.Utility;
using Esmart.Framework.Caching;
namespace Esmart.Framework.Cache
{
    public class LocalCacheProvider : ICacheProvider
    {
        public virtual object Get(string key)
        {
            return Esmart.Framework.Caching.Caching.Get(key);
        }

        public virtual void Set(string key, object value, int minutes, bool isAbsoluteExpiration, Action<string, object, string> onRemove)
        {
            Esmart.Framework.Caching.Caching.Set(key, value, minutes, isAbsoluteExpiration, (k, v, reason) =>
                {
                    if (onRemove != null)
                        onRemove(k, v, reason.ToString());
                });
        }

        public virtual void Remove(string key)
        {
            Esmart.Framework.Caching.Caching.Remove(key);
        }

        public virtual void Clear(string keyRegex)
        {
            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Key.ToString();
                if (Regex.IsMatch(key, keyRegex, RegexOptions.IgnoreCase))
                    keys.Add(key);
            }
    
            for (int i = 0; i < keys.Count; i++)
            {
                HttpRuntime.Cache.Remove(keys[i]);
            }
            
 
        }
    }
}
