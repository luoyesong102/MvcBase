using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class LogHelper<T>
    {
       private static readonly Dictionary<string, Action<UserLiteDto, T>> _logAppend = new Dictionary<string, Action<UserLiteDto, T>>();
       internal static Action<UserLiteDto, T> LogInstance(string key, Action<UserLiteDto, T> action)
       {
           if(!_logAppend.ContainsKey(key))
           {
                _logAppend.Add(key,null);
           }
           if(_logAppend[key]==null)
           {
               _logAppend[key] = action;
           }
           return _logAppend[key];
       }

       public static Action<UserLiteDto, T> LogAction(string key)
       {
          return _logAppend.ContainsKey(key)?_logAppend[key]:null;
       }

       public static void RemoveAction(string key)
       {
           if (_logAppend.ContainsKey(key))
           {
               _logAppend.Remove(key);
           }
       }
    }
}
