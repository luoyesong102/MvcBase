using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Exceptions
{
    public class ReflectHelper
    {
        /// <summary>
        /// 对值类型设置初始值
        /// </summary>
        /// <param name="properinfo"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        private static void SetValueTypeOrString(System.Reflection.PropertyInfo properinfo, object obj, string key)
        {
            string findKey = System.Web.HttpContext.Current.Request[key];

            if (string.IsNullOrEmpty(findKey))
            {
                return;
            }

            var propertyType = properinfo.PropertyType;
            if (propertyType.IsGenericType)
            {
                propertyType = properinfo.PropertyType.GetGenericArguments()[0];
            }

            if (propertyType.IsEnum)
            {
                dynamic value = Enum.Parse(propertyType, findKey);
                properinfo.SetValue(obj, value, null);
            }
            else
            {
                dynamic value = Convert.ChangeType(findKey, propertyType);
                properinfo.SetValue(obj, value, null);
            }

        }

        /// <summary>
        /// 设置IEnumerable结果
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="properinfo"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        private static void SetIEnumerableValueType(Type argument, System.Reflection.PropertyInfo properinfo, object obj, string key)
        {

            dynamic dynList = Activator.CreateInstance(properinfo.PropertyType);

            string findkey = key + "[]";

            string findValue = System.Web.HttpContext.Current.Request[findkey];

            if (string.IsNullOrEmpty(findValue))
            {
                return;
            }

            string[] strArray = findValue.Split(',');

            foreach (string stra in strArray)
            {
                if (!string.IsNullOrEmpty(stra))
                {
                    dynamic strasas = Convert.ChangeType(stra, argument);
                    dynList.Add(strasas);
                }
            }

            properinfo.SetValue(obj, dynList, null);
        }
        /// <summary>
        /// 对IEnumerable设置初始值
        /// </summary>
        /// <param name="properinfo"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="keys"></param>
        private static void SetIEnumerableClass(System.Reflection.PropertyInfo properinfo, object obj, string key, string[] keys)
        {

            var arguments = properinfo.PropertyType.GetGenericArguments();

            var properinfos = arguments[0].GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            dynamic objList = Activator.CreateInstance(properinfo.PropertyType);

            string findkey = null;

            for (int i = 0; i < keys.Length; i++)
            {
                findkey = key + "[" + i + "]";

                bool isExistes = false;

                foreach (var info in properinfos)
                {
                    string findlistkey = findkey + "[" + info.Name + "]";

                    findlistkey = keys.FirstOrDefault(a => a.ToLower().Trim() == findlistkey.ToLower().Trim());

                    if (string.IsNullOrEmpty(findlistkey))
                    {
                        continue;
                    }

                    string findvalue = System.Web.HttpContext.Current.Request[findlistkey];

                    if (string.IsNullOrEmpty(findvalue))
                    {
                        continue;
                    }

                    isExistes = true;

                    break;
                }

                if (isExistes)
                {
                    dynamic objvalue = Activator.CreateInstance(arguments[0]);

                    SetValue(objvalue, findkey, keys);

                    objList.Add(objvalue);
                }

            }
            properinfo.SetValue(obj, objList, null);
        }


        private static bool IsExistesForm(object obj, string key, string[] keys)
        {
            string findKey = null;
            var properinfos = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            bool isExistes = false;
            foreach (var properinfo in properinfos)
            {
                if (string.IsNullOrEmpty(key))
                {
                    findKey = properinfo.Name;
                }
                else
                {
                    findKey = key + "[" + properinfo.Name + "]";
                }

                if (keys.FirstOrDefault(a => a.Trim().ToLower() == findKey.Trim().ToLower()) != null)
                {
                    isExistes = true;
                    break;
                }
            }

            return isExistes;
        }

        public static void SetValue(object obj, string key, string[] keys)
        {
            string findKey = null;

            var properinfos = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var properinfo in properinfos)
            {
                if (string.IsNullOrEmpty(key))
                {
                    findKey = properinfo.Name;
                }
                else
                {
                    findKey = key + "[" + properinfo.Name + "]";
                }

                if (properinfo.PropertyType.IsValueType || properinfo.PropertyType == typeof(string))
                {
                    SetValueTypeOrString(properinfo, obj, findKey);
                    continue;
                }

                Type iEnumerable = properinfo.PropertyType.GetInterface("IEnumerable", false);

                if (iEnumerable != null)
                {
                    if (properinfo.PropertyType.IsGenericType)
                    {
                        var arguments = properinfo.PropertyType.GetGenericArguments();

                        if (arguments[0].IsValueType || arguments[0] == typeof(string))
                        {
                            SetIEnumerableValueType(arguments[0], properinfo, obj, findKey);
                            continue;
                        }
                        else
                        {
                            SetIEnumerableClass(properinfo, obj, findKey, keys);
                            continue;
                        }
                    }
                }

                if (properinfo.PropertyType.IsClass && iEnumerable == null)
                {
                    dynamic model = Activator.CreateInstance(properinfo.PropertyType);

                    if (IsExistesForm(model, findKey, keys))
                    {
                        SetValue(model, findKey, keys); 
                        properinfo.SetValue(obj, model, null);
                    }
                }
            }
        }
    }


}
