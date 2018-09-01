using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Esmart.Framework.Configuration
{
    public class PathString  
    {
        /// <summary>
        /// 解析路径字符串
        /// </summary>
        /// <param name="formattedString"></param>
        /// <returns></returns>
        public static string Parse(string formattedString)
        {
            Regex regex = new Regex("{BaseDirectry}");
            var ary = regex.Split(formattedString);

            var result = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var item in ary)
            {
                result = Path.Combine(result, item.Replace("/", ""));
            }
            return result;

            //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //return Regex.Replace(formattedString, "{BaseDirectry}", baseDirectory);
        }
    }
}
