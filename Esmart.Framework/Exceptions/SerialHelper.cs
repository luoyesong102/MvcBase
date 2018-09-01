using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Esmart.Framework.Exceptions
{
    public class SerialHelper
    {
      

        public static void SetDefaultValue<T>(T item)
        {
            Type type = item.GetType();
            PropertyInfo[] propers = type.GetProperties();
            for (int i = 0; i < propers.Length; i++)
            {
                var pItem = propers[i];
                if (pItem.PropertyType == typeof(int))
                {
                    pItem.SetValue(item, i, null);
                }
                else if (pItem.PropertyType == typeof(string))
                {
                    pItem.SetValue(item, "test" + i.ToString(), null);
                }
                else if (pItem.PropertyType == typeof(DateTime))
                {
                    pItem.SetValue(item, DateTime.Now, null);
                }
            }
        }
    }
}
