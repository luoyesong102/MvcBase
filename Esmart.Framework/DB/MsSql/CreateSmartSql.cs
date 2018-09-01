using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Esmart.Framework.Model;

namespace Esmart.Framework.DB
{
    internal partial class CreateSQl
    {
        static long index = 0;

        /// <summary>
        /// 前缀查询(or and >= 等)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="list"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string Query(object obj, ref List<DbParameter> list, string prefix)
        {
            StringBuilder strWhere = new StringBuilder();

            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }

            if (!obj.GetType().IsClass)
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in proertyinfos)
            {
                if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface) && info.PropertyType != typeof(string))
                {
                    continue;
                }
                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                index++;
                string  parameterName="@" + info.Name + index;

                strWhere.Append(string.Format(" {0} {1}={2} ", prefix, GetNameByType(info),parameterName));

                list.Add(new SqlParameter(parameterName, value));
            }
            return strWhere.ToString();
        }


        public static string Query(object obj, ref List<DbParameter> list)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }

            if (!obj.GetType().IsClass)
            {
                return string.Empty;
            }
            Type type = obj.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            StringBuilder strWhere = new StringBuilder();
         
            if (obj == null)
            {
                return strWhere.ToString();
            }

            foreach (PropertyInfo info in proertyinfos)
            {

                var attributes = info.GetCustomAttributes(typeof(QueryIgnoreAttribute), false);

                if (attributes.Count() > 0)
                {
                    continue;
                }

                if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface) && info.PropertyType != typeof(string))
                {
                    continue;
                }

                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                index++;

                string parameterName = "@" + info.Name + index;

                var orQuery = info.GetCustomAttributes(typeof(OrQuery), false);

                string orAnd = " and";

                if (orQuery.Count() > 0)
                {
                    orAnd = " or ";
                }
                attributes = info.GetCustomAttributes(typeof(GreaterThan), false);
                if (attributes.Count() != 0)
                {
                    strWhere.Append(string.Format(" {0} {1}>={2} ", orAnd, GetNameByType(info), parameterName));
                    list.Add(new SqlParameter(parameterName, value));
                    continue;
                }

                attributes = info.GetCustomAttributes(typeof(LessThan), false);
                if (attributes.Count() != 0)
                {
                    strWhere.Append(string.Format(" {0} {1}<={2} ", orAnd, GetNameByType(info), parameterName));
                    list.Add(new SqlParameter(parameterName, value));
                    continue;
                }


                attributes = info.GetCustomAttributes(typeof(FuzzyQuery), false);
                if (attributes.Count() != 0)
                {
                    strWhere.Append(string.Format(" {0} {1} like {2} ", orAnd, GetNameByType(info), parameterName));
                    list.Add(new SqlParameter(parameterName, "%" + value + "%"));
                    continue;
                }

                if (info.PropertyType.IsArray)
                {
                    var array = info.GetValue(obj, null) as dynamic;

                    if (value != null && array.Length > 0)
                    {
                        if (array[0].GetType() == typeof(string))
                        {
                            strWhere.Append(string.Format("  {0} {1} in('{2}')", orAnd, GetNameByType(info), string.Join("','", array)));
                        }
                        else
                        {
                            strWhere.Append(string.Format("  {0} {1} in({2})", orAnd, GetNameByType(info), string.Join(",", array)));
                        }
                        continue;
                    }
                }
                else if (info.PropertyType.GetInterface("IEnumerable") != null)
                {
                    var objvalue = info.GetValue(obj, null) as dynamic;

                    var count = 0;

                    if (info.PropertyType.GetField("Length") != null)
                    {
                        count = objvalue.Count;
                    }
                    else if (info.PropertyType.GetField("Count") != null)
                    {
                        count = objvalue.Count;
                    }
                    if (count > 0)
                    {
                        if (info.PropertyType.DeclaringType != null && info.PropertyType.DeclaringType.IsClass)
                        {
                            strWhere.Append(string.Format("  {0} {1} in('{2}')", orAnd, GetNameByType(info), string.Join("','", objvalue)));
                        }
                        else
                        {
                            strWhere.Append(string.Format("  {0} {1} in({2})", orAnd, GetNameByType(info), string.Join(",", objvalue)));
                        }
                        continue;
                    }
                }
                strWhere.Append(string.Format(" {0} {1}={2} ", orAnd, GetNameByType(info), parameterName));
                list.Add(new SqlParameter(parameterName, value));
            }



            return strWhere.ToString();
        }



        /// <summary>
        /// 前缀查询(or and >= 等)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="list"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string LessQuery(object obj, ref List<DbParameter> list, string prefix)
        {
            StringBuilder strWhere = new StringBuilder();

            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }

            if (!obj.GetType().IsClass)
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in proertyinfos)
            {

                if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface) && info.PropertyType != typeof(string))
                {
                    continue;
                }
                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                index++;
                string parameterName = "@" + info.Name + index;

                strWhere.Append(string.Format(" {0} {1}<={2} ", prefix, GetNameByType(info),parameterName));

                list.Add(new SqlParameter(parameterName, value));
            }

            return strWhere.ToString();
        }


        /// <summary>
        /// 前缀查询(or and >= 等)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="list"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GreateQuery(object obj, ref List<DbParameter> list, string prefix)
        {
            StringBuilder strWhere = new StringBuilder();
           
            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }

            if (!obj.GetType().IsClass)
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in proertyinfos)
            {

                if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface) && info.PropertyType != typeof(string))
                {
                    continue;
                }
                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                index++;
                string parameterName = "@" + info.Name + index;
                strWhere.Append(string.Format(" {0} {1}>={2} ", prefix, GetNameByType(info), parameterName));

                list.Add(new SqlParameter(parameterName, value));
            }

            return strWhere.ToString();
        }


        public static string FuzzyQuery(object obj, ref List<DbParameter> list, string prefix)
        {
            StringBuilder strWhere = new StringBuilder();
 
            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }

            if (!obj.GetType().IsClass)
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in proertyinfos)
            {

                if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface || info.PropertyType.IsValueType || info.PropertyType.IsGenericType) && info.PropertyType != typeof(string))
                {
                    continue;
                }
                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                index++;

                string parameterName = "@" + info.Name + index;

                strWhere.Append(string.Format(" {0} {1} like {2} ", prefix, GetNameByType(info),parameterName));

                list.Add(new SqlParameter(parameterName, "%" + value + "%"));
            }

            return strWhere.ToString();
        }


        public static string InQuery(object obj, string prefix, ref List<DbParameter> list)
        {
            StringBuilder strWhere = new StringBuilder();

            if (obj == null)
            {
                return string.Empty;
            }
            if (obj.GetType() == typeof(string))
            {
                return obj.ToString();
            }

            if (!obj.GetType().IsClass)
            {
                return string.Empty;
            }

            Type type = obj.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in proertyinfos)
            {
                var value = info.GetValue(obj, null) as dynamic;

                if (value == null)
                {
                    continue;
                }

                if (info.PropertyType.GetInterface("IEnumerable") != null && info.PropertyType != typeof(string))
                {
                    var objvalue = info.GetValue(obj, null) as dynamic;

                    string name = "@" + GetNameByType(info);

                    StringBuilder strin = new StringBuilder();
                    int i = 0;
                    foreach (dynamic infoValue in objvalue)
                    {
                        string parName = name + i;
                        if (strin.Length == 0)
                        {
                            strin.Append(parName);
                        }
                        else
                        {
                            strin.Append("," + parName);
                        }
                        list.Add(new SqlParameter(parName, infoValue));
                        i++;
                    }
                    if (strin.Length != 0)
                    {
                        strWhere.Append(string.Format("  {0} {1} in({2})", prefix, GetNameByType(info), strin.ToString()));
                    }
                }
            }
            return strWhere.ToString();
        }

        public static string InTableQuery<Tobj>(string prefix, string mainId, object obj, ref List<DbParameter> list, string tableId = null)
        {
            tableId = tableId == null ? mainId : tableId;

            return string.Format(" {0} {1} in ( select  {2} from {3} {4} )", prefix, tableId, mainId, GetNameByType(typeof(Tobj)), GetWhereString(obj, ref list));
        }



        public static string UpdateObjectBySmart<Tobj>(Tobj obj, ref List<DbParameter> listParam)
        {
            StringBuilder tableCols = new StringBuilder();
            Type type = obj.GetType();
            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in proertyinfos)
            {
                object value = info.GetValue(obj, null);
                if (IsDefault(value))
                {
                    continue;
                }

                object[] objs = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);

                if (objs.Length > 0)
                {
                    continue;
                }

                string name = GetNameByType(info);

                listParam.Add(new SqlParameter("@" + name, value));

                if (tableCols.Length == 0)
                {
                    tableCols.Append(string.Format(" {0}=@{1}", name, name));
                }
                else
                {
                    tableCols.Append(string.Format(",{0}=@{1}", name, name));
                }
            }
            return string.Format("UPDATE {0}  SET {1} ", GetNameByType(type), tableCols);
        }
    }

}
