using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Esmart.Framework.Model;

namespace Esmart.Framework.DB
{
    internal partial class CreateSQl
    {

        public static string IsDelete = "IsDelete";

        static long ParIndex = 0;

        /// <summary>
        /// 生产标准insert语句
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string InsertObjectString<Tobj>(Tobj obj, ref List<DbParameter> listParam)
        {
            StringBuilder tableCols = new StringBuilder();
            StringBuilder valueCols = new StringBuilder();

            Type type = obj.GetType();
            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> listCol = new List<string>();
            List<string> listValue = new List<string>();
            foreach (PropertyInfo info in proertyinfos)
            {
                object value = info.GetValue(obj, null);
                if (IsDefault(value))
                {
                    continue;
                }

                var primarys = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);

                if (primarys.Count() > 0 && IsPrimaryDefault(value))
                {
                    continue;
                }

                string name = GetNameByType(info);
                string parName = "@" + name+ParIndex;
                ParIndex++;
                listParam.Add(new SqlParameter(parName, value));
                listCol.Add(name);
                listValue.Add(parName);
            }
            return string.Format("INSERT INTO {0}({1}) VALUES({2}) select @@IDENTITY ", GetNameByType(type), string.Join(",", listCol), string.Join(",", listValue));
        }

        public static string InsertListObjectString<Tobj>(IEnumerable<Tobj> objList, ref List<DbParameter> listParam)
        {
            StringBuilder sql = new StringBuilder();
            foreach (Tobj obj in objList)
            {
                StringBuilder tableCols = new StringBuilder();
                StringBuilder valueCols = new StringBuilder();
                List<string> listCol = new List<string>();
                List<string> listValue = new List<string>();
                Type type = obj.GetType();
                PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo info in proertyinfos)
                {
                    object value = info.GetValue(obj, null);
                    if (IsDefault(value))
                    {
                        continue;
                    }

                    var primarys = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);

                    if (primarys.Count() > 0 && IsPrimaryDefault(value))
                    {
                        continue;
                    }
                    string name = GetNameByType(info);
                    string parName = "@" + name + ParIndex;
                    listParam.Add(new SqlParameter(parName, value));
                    listCol.Add(name);
                    listValue.Add(parName);
                    ParIndex++;
                }
                sql.Append(string.Format(" INSERT INTO {0}({1}) VALUES({2}) select @@IDENTITY asID ", GetNameByObj(obj), string.Join(",", listCol), string.Join(",", listValue)));
            }
            return string.Format(" begin tran {0} if @@error<>0 begin  rollback tran end else  begin commit tran end", sql.ToString());
        }


        public static string UpdateObjectString<Tobj>(Tobj obj, ref List<DbParameter> listParam, bool isUserDefalut = true,bool userWhere=true)
        {
            StringBuilder tableCols = new StringBuilder();
            Type type = obj.GetType();
            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in proertyinfos)
            {
                object value = info.GetValue(obj, null);
                if (isUserDefalut)
                {
                    if (IsDefault(value))
                    {
                        continue;
                    }
                }

                object[] objs = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);

                if (objs.Length > 0)
                {
                    continue;
                }

                string name = GetNameByType(info);
                ParIndex++;
                string parName = "@" + name+ParIndex;

                if (value == null)
                {
                    listParam.Add(new SqlParameter(parName, DBNull.Value));
                }
                else
                {
                    listParam.Add(new SqlParameter(parName, value));

                }
                if (tableCols.Length == 0)
                {
                    tableCols.Append(string.Format(" {0}={1}", name, parName));
                }
                else
                {
                    tableCols.Append(string.Format(",{0}={1}", name, parName));
                }
            }
            if (userWhere)
            {
                return string.Format("UPDATE {0}  SET {1}  {2}", GetNameByType(type), tableCols, GetIdentityWhereString(obj, ref listParam));
            }
            else
            {
                return string.Format("UPDATE {0}  SET {1}  ", GetNameByType(type), tableCols);
            }
          
        }





        public static string UpdateListObjectString<Tobj>(IEnumerable<Tobj> objList, ref List<DbParameter> listParam, bool isUserDefalut = true)
        {
            StringBuilder sql = new StringBuilder();
            int i = 0;
            foreach (Tobj obj in objList)
            {
                StringBuilder tableCols = new StringBuilder();
                Type type = obj.GetType();
                PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo info in proertyinfos)
                {
                    object value = info.GetValue(obj, null);
                    if (isUserDefalut)
                    {
                        if (IsDefault(value))
                        {
                            continue;
                        }
                    }
                    object[] objs = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);

                    if (objs.Length > 0)
                    {
                        continue;
                    }

                    string name = GetNameByType(info);

                    string parName = "@" + name + i;

                    if (value == null)
                    {
                        listParam.Add(new SqlParameter(parName, DBNull.Value));
                    }
                    else
                    {
                        listParam.Add(new SqlParameter(parName, value));

                    }

                    if (tableCols.Length == 0)
                    {
                        tableCols.Append(string.Format(" {0}={1}", name, parName));
                    }
                    else
                    {
                        tableCols.Append(string.Format(",{0}={1}", name, parName));
                    }
                    i++;
                }
                sql.AppendLine(string.Format(" UPDATE {0}  SET {1}  {2}", GetNameByType(typeof(Tobj)), tableCols, GetIdentityWhereString(obj, ref listParam, i)));
            }
            return string.Format(" begin tran {0} if @@error<>0 begin  rollback tran end else  begin commit tran end ", sql.ToString());
        }



        public static string DeleteObjectString<Tobj>(object id, ref List<DbParameter> listParam)
        {
            Type type = typeof(Tobj);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return string.Format(" UPDATE {0}  SET IsDelete=1   {1}", GetNameByType(type), FindIdentityByID<Tobj>(id, ref listParam));
            }

            return string.Format(" DELETE {0}  {1}", GetNameByType(type), FindIdentityByID<Tobj>(id, ref listParam));
        }

        public static string DeleteObjectString<Tobj>()
        {
            Type type = typeof(Tobj);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return string.Format(" UPDATE {0}  SET IsDelete=1   ", GetNameByType(type));
            }

            return string.Format(" DELETE {0} ", GetNameByType(type));
        }

        public static string DeleteListObjectString<Tobj>(IEnumerable<object> ids, ref List<DbParameter> listParam)
        {
            StringBuilder sql = new StringBuilder();
            int i = 0;

            Type type = typeof(Tobj);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            bool isDeleteFiled = false;
            if (property != null)
            {
                isDeleteFiled = true;
            }

            foreach (object obj in ids)
            {
                if (isDeleteFiled)
                {
                    sql.AppendLine(string.Format(" UPDATE {0}  SET IsDelete=1   {1}", GetNameByType(type), FindIdentityByID<Tobj>(obj, ref listParam, i)));
                }
                else
                {
                    sql.AppendLine(string.Format(" DELETE {0}   {1}", GetNameByType(type), FindIdentityByID<Tobj>(obj, ref listParam, i)));
                }
                i++;
            }
            return string.Format(" begin tran {0} if @@error<>0 begin  rollback tran end else  begin commit tran end ", sql.ToString());
        }




        public static object GetNameByType(Type type, object where = null)
        {
            object[] objs = type.GetCustomAttributes(typeof(TableNameAttribute), false);
            if (objs != null && objs.Length > 0)
            {
                TableNameAttribute tableName = (TableNameAttribute)objs[0];
                if (tableName != null)
                {
                    if (!tableName.IsUserDateCreateTable)
                    {
                        return tableName.Name;
                    }
                    else
                    {
                        string[] tableNames = tableName.Name.Split(new Char[] { '{', '}' });
                        if (tableNames.Length == 3)
                        {
                            if (where != null)
                            {
                                var property = where.GetType().GetProperty("CreateTime", BindingFlags.IgnoreCase);
                                if (property != null)
                                {
                                    DateTime dt = (DateTime)property.GetValue(where, null);

                                    return string.Format("{0}{1}{2}", tableNames[0], dt.ToString(tableNames[1]), tableNames[2]);
                                }
                            }
                            return string.Format("{0}{1}{2}", tableNames[0], DateTime.Now.ToString(tableNames[1]), tableNames[2]);
                        }

                    }
                }
            }

            return type.Name;
        }


        private static object GetNameByObj(object obj)
        {
            object[] objs = obj.GetType().GetCustomAttributes(typeof(TableNameAttribute), false);

            if (objs != null && objs.Length > 0)
            {
                TableNameAttribute tableName = (TableNameAttribute)objs[0];
                if (tableName != null)
                {
                    if (!tableName.IsUserDateCreateTable)
                    {
                        return tableName.Name;
                    }
                    else
                    {
                        string[] tableNames = tableName.Name.Split(new Char[] { '{', '}' });

                        var fileinfo = obj.GetType().GetProperty("CreateTime");

                        DateTime dt = DateTime.Now;
                        if (fileinfo != null)
                        {
                            dt = Convert.ToDateTime(fileinfo.GetValue(obj, null));
                        }
                        if (tableNames.Length == 3)
                        {
                            return string.Format("{0}{1}{2}", tableNames[0], dt.ToString(tableNames[1]), tableNames[2]);
                        }
                    }
                }
            }
            return obj.GetType().Name;
        }



        public static string CreateListString<Tobj>()
        {
            StringBuilder strWhere = new StringBuilder();
            Type type = typeof(Tobj);
            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            string where = string.Empty;

            if (property != null)
            {
                where = " where IsDelete=0";
            }
            return string.Format(" SELECT * FROM  {0} {1} ", GetNameByType(type) + "(nolock)", where);
        }



        public static string CreateListString<Tobj>(object model, ref  List<DbParameter> list)
        {
            StringBuilder strWhere = new StringBuilder();
            Type type = typeof(Tobj);
            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            string where = GetWhereString(model, ref list);

            if (property != null)
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = " where  IsDelete=0";
                }
                else
                {
                    where += " and  IsDelete=0";
                }
            }
            return string.Format(" SELECT * FROM  {0} {1} ", GetNameByType(type) + "(nolock)", where);
        }

        public static string CreateListStringByWhere<Tobj>(string where)
        {
            Type type = typeof(Tobj);
            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                if (!string.IsNullOrEmpty(where))
                {
                    where = where + " and IsDelete=0 ";
                }
                else
                {
                    where = " where  IsDelete=0 ";
                }

            }

            StringBuilder strWhere = new StringBuilder();

            return string.Format(" SELECT * FROM  {0} {1} ", GetNameByType(type) + "(nolock)", where);
        }


        public static string CreatePageString<Tobj>(DBPage page, ref List<DbParameter> list)
        {


            StringBuilder strWhere = new StringBuilder();
            Type type = typeof(Tobj);

            string orderBy = string.Empty;

            string strsql = string.Empty;

            if (!string.IsNullOrEmpty(page.OrderBy))
            {
                orderBy = page.OrderBy;
            }

            string where = GetWhereString(page.Where, ref list);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = " where IsDelete=0";
                }
                else
                {
                    where += " and IsDelete=0";
                }
            }
            if (page.PageIndex <= 1)
            {
                return string.Format("SELECT TOP {0} * FROM {1} {2} ORDER BY {3} {4}", page.PageSize, GetNameByType(type, page.Where) + "(nolock)", where, orderBy, page.SortCol);
            }
            else
            {
                strsql = "SELECT TOP {0} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1} {6}) AS RowNumber,*  from {2} {3}) as A  where RowNumber>{4} order by {5} {6}";
            }
            return string.Format(strsql, page.PageSize, orderBy, GetNameByType(type, page.Where) + "(nolock)", where, (page.PageIndex - 1) * page.PageSize, orderBy, page.SortCol);
        }


        public static string CreatePageCountString<Tobj>(DBPage page, ref List<DbParameter> list)
        {

            string where = null;
            Type type = typeof(Tobj);

            where = GetWhereString(page.Where, ref list);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                if (string.IsNullOrEmpty(where))
                {
                    where = " where IsDelete=0";
                }
                else
                {
                    where += " and IsDelete=0";
                }
            }
            return string.Format(" select count(*)  from {0} {1}", GetNameByType(typeof(Tobj)) + "(nolock)", where);
        }





        public static string CreateSelectCount<Tobj>(string where)
        {
            return string.Format(" SELECT count(*) from {0} {1}", GetNameByType(typeof(Tobj)) + "(nolock)", where);
        }


        public static string CreateModelString<Tobj>(object obj, ref List<DbParameter> list)
        {
            if (obj == null)
            {
                throw new Exception("对象不能为空");
            }

            StringBuilder strWhere = new StringBuilder();
            Type type = typeof(Tobj);

            string where = null;

            if (obj.GetType().IsClass && obj.GetType() != typeof(string))
            {
                where = GetWhereString(obj, ref list);
            }

            if (obj.GetType().IsValueType)
            {
                where = FindIdentityByID<Tobj>(obj, ref list);
            }
            if (obj.GetType() == typeof(string) && obj.ToString().IndexOf("where", StringComparison.OrdinalIgnoreCase) != -1)
            {
                where = obj.ToString();
            }

            if (string.IsNullOrEmpty(where))
            {
                where = FindIdentityByID<Tobj>(obj, ref list);
            }

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                where += " and IsDelete=0";
            }
            return string.Format(" SELECT * FROM {0} {1}", GetNameByType(type) + "(nolock)", where);
        }


        public static string CreateDbParameter<Tobj>(Tobj obj, SqlString sqlModel, ref List<DbParameter> list)
        {
            if (obj == null)
            {
                return null;
            }
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine(sqlModel.SqlContent);

            Type type = obj.GetType();
            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in proertyinfos)
            {
                var objValue = info.GetValue(obj, null);
                if (objValue == null)
                {
                    continue;
                }
                string infoName = GetNameByType(info);
                var model = sqlModel.SqlParam.Find(a => a.ParamName.Trim() == infoName.Trim());
                if (model != null)
                {
                    strSql.AppendLine(model.ParamContent);
                    list.Add(new SqlParameter("@" + infoName, objValue));
                }
            }
            return strSql.ToString();
        }


        public static string CreateDbParameter<Tobj>(Tobj obj, SqlContent sqlModel, ref List<DbParameter> list, bool userSqlContent)
        {
            if (obj == null)
            {
                return sqlModel.Content;
            }

            string where = string.Empty;
            if (!string.IsNullOrEmpty(sqlModel.Parameter))
            {
                where = sqlModel.Parameter;
            }
            if (userSqlContent)
            {
                return CreateSQl.CreateSqlBySqlPar(sqlModel.Content + where, obj, ref list);
            }
            else
            {
                where = CreateSQl.CreateSqlBySqlPar(where, obj, ref list);
                return string.Format("{0} {1}", sqlModel.Content, where);
            }
        }



        private static string GetIdentityWhereString<Tobj>(Tobj obj, ref List<DbParameter> list, int index = 0)
        {
            StringBuilder strWhere = new StringBuilder();
            PropertyInfo[] proertyinfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (proertyinfos.FirstOrDefault(a => a.Name.ToLower() == IsDelete.ToLower()) != null)
            {
                strWhere.Append(" where IsDelete=0 ");
            }
            else
            {
                strWhere.Append(" where 1=1 ");
            }
            bool isExistsPrimarykeyAttrbute = false;

            foreach (PropertyInfo info in proertyinfos)
            {
                var atrribute = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);
                if (atrribute != null && atrribute.Length > 0)
                {
                    var data = info.GetValue(obj, null);
                    if (data == null)
                    {
                        continue;
                    }
                    string name = GetNameByType(info);

                    if ((info.PropertyType.IsArray || info.PropertyType.IsGenericType) && info.PropertyType.IsClass)
                    {
                        var objvalue = info.GetValue(obj, null) as dynamic;
                        if (objvalue != null && objvalue.Length > 0)
                        {
                            if (info.PropertyType.DeclaringType != null && info.PropertyType.DeclaringType.IsClass)
                            {
                                strWhere.Append(string.Format("  and {0} in('{1}')", GetNameByType(info), string.Join("','", objvalue)));
                            }
                            else
                            {
                                strWhere.Append(string.Format("  and {0} in({1})", GetNameByType(info), string.Join(",", objvalue)));
                            }
                            continue;
                        }
                    }

                    string parameterName = "@" + name + "identity" + index;

                    strWhere.Append(string.Format(" and {0}={1}", name, parameterName));

                    list.Add(new SqlParameter(parameterName, data));

                    isExistsPrimarykeyAttrbute = true;

                }

            }

            if (!isExistsPrimarykeyAttrbute)
            {
                throw new Esmart.Framework.Model.TpoBaseException("主键不能为空，请联系开发人员");
            }
            return strWhere.ToString();
        }


        private static string FindIdentityByID<Tobj>(object id, ref List<DbParameter> list, int index = 0)
        {
            StringBuilder strWhere = new StringBuilder();
            PropertyInfo[] proertyinfos = typeof(Tobj).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo info in proertyinfos)
            {
                var atrribute = info.GetCustomAttributes(typeof(PrimarykeyAttrbute), false);
                if (atrribute != null && atrribute.Length > 0)
                {

                    string name = GetNameByType(info);

                    string parameterName = "@" + name + "identity" + index;

                    strWhere.Append(string.Format(" where {0}={1}", name, parameterName));

                    list.Add(new SqlParameter(parameterName, id));

                    break;

                }

            }

            if (strWhere.Length == 0)
            {
                throw new Esmart.Framework.Model.TpoBaseException("删除指定的唯一key不存在");
            }
            return strWhere.ToString();
        }

        public static string GetNameByType(PropertyInfo info)
        {

            var attributes = info.GetCustomAttributes(typeof(ColNameAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                ColNameAttribute colName = attributes[0] as ColNameAttribute;
                if (colName != null)
                {
                    return colName.Name ?? info.Name;
                }
                attributes = info.GetCustomAttributes(typeof(TableNameAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    TableNameAttribute tableName = attributes[0] as TableNameAttribute;
                    if (tableName != null)
                    {
                        if (!tableName.IsUserDateCreateTable)
                        {
                            return tableName.Name;
                        }
                        else
                        {
                            string[] tableNames = tableName.Name.Split(new Char[] { '{', '}' });
                            if (tableNames.Length == 3)
                            {
                                return string.Format("{0}{1}{2}", tableNames[0], DateTime.Now.ToString(tableNames[1]), tableNames[2]);
                            }

                        }
                    }
                }
            }

            return info.Name;
        }


        private static string GetWhereString(object obj, ref List<DbParameter> list)
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
            strWhere.Append("  where 1=1 ");

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

                if ((info.PropertyType.IsArray || info.PropertyType.IsGenericType) && info.PropertyType.IsClass)
                {
                    var objvalue = info.GetValue(obj, null) as dynamic;
                    if (objvalue != null && objvalue.Length > 0)
                    {
                        if (info.PropertyType.DeclaringType != null && info.PropertyType.DeclaringType.IsClass)
                        {
                            strWhere.Append(string.Format("  and {0} in('{1}')", GetNameByType(info), string.Join("','", objvalue)));
                        }
                        else
                        {
                            strWhere.Append(string.Format("  and {0} in({1})", GetNameByType(info), string.Join(",", objvalue)));
                        }
                        continue;
                    }
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

                var orQuery = info.GetCustomAttributes(typeof(OrQuery), false);

                string orAnd = " and";

                if (orQuery.Count() > 0)
                {
                    orAnd = " or ";
                }

                index++;

                string parameterName = "@" + info.Name + index;

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

                strWhere.Append(string.Format(" {0} {1}={2} ", orAnd, GetNameByType(info), parameterName));
                list.Add(new SqlParameter(parameterName, value));
            }

            return strWhere.ToString();
        }


        public static List<DbParameter> GetWhere(object obj)
        {
            List<DbParameter> list = new List<DbParameter>();
            if (obj == null)
            {
                return list;
            }

            var type = obj.GetType();

            if (type.IsValueType)
            {
                throw new TpoBaseException("查询条件不能为值类型");
            }

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in proertyinfos)
            {
                var value = info.GetValue(obj, null);
                if (value == null)
                {
                    continue;
                }
                SqlParameter par = new SqlParameter("@" + GetNameByType(info), value);
                list.Add(par);
            }
            return list;
        }





        public static bool IsPrimaryDefault(object obj)
        {
            if (obj.GetType().IsValueType)
            {
                return Convert.ToInt64(obj) == 0;
            }
            return false;
        }


        public static bool IsDefault(object obj)
        {
            if (obj == null)
            {
                return true;
            }
            if (typeof(DateTime) == obj.GetType())
            {
                return Convert.ToDateTime(obj) == DateTime.MinValue;
            }
            return false;
        }

    }


    internal partial class CreateSQl
    {
        public static string CreateSqlBySqlPar(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return string.Empty;
            }

            sql = sql.Replace("where", " where 1=1  and ");

            Regex mytregex = new Regex(@"\S+\s+(not\s*in|in)\s*\(\s*@null\s*\)");

            var newsql = mytregex.Replace(sql, "  value=@null ");

            mytregex = new Regex(@"(and|or)\s+(\S+\s*(=|like|>|>=|<|<=|!=|<>)\s*@null)");

            newsql = mytregex.Replace(newsql, "");

            mytregex = new Regex(@"(and|or)\s+(\S+\s*(=|like|>|>=|<|<=|!=|<>)\s*@null)");

            newsql = mytregex.Replace(newsql, " and 1=1 ");


            mytregex = new Regex(@"(\S+\s*(=|like|>|>=|<|<=)\s*@null)");

            newsql = mytregex.Replace(newsql, " 1=1 ");

            mytregex = new Regex(@"and\s*1=1");

            newsql = mytregex.Replace(newsql, "");

            return newsql;
        }


        public static string CreateSqlBySqlPar<T>(string sql, T model, ref List<DbParameter> listParam)
        {
            if (model == null || string.IsNullOrEmpty(sql))
            {
                return string.Empty;
            }

            sql = sql.ToLower().Replace("(", "( ").Replace(")", " )").Replace(" ", "    ");

            var mytregex = new Regex(@"@\S+");


            List<string> parName = new List<string>();

            var matches = mytregex.Matches(sql);

            for (int i = 0; i < matches.Count; i++)
            {
                string key = matches[i].ToString().Trim();
                if (!parName.Contains(key))
                {
                    parName.Add(key);
                }
            }
            Type type = model.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var info in properties)
            {
                var value = info.GetValue(model, null);
                if (value == null)
                {
                    continue;
                }
                var name = "@" + CreateSQl.GetNameByType(info).ToLower();

                if ((info.PropertyType.IsArray || info.PropertyType.IsGenericType) && info.PropertyType.IsClass)
                {
                    var objvalue = info.GetValue(model, null) as dynamic;
                    if (objvalue != null && objvalue.Count > 0)
                    {
                        int i = 0;
                        List<string> list = new List<string>();
                        foreach (dynamic obj in objvalue)
                        {
                            string par = name + i;
                            listParam.Add(new SqlParameter(par, obj));
                            list.Add(par);
                            i++;
                        }
                        sql = sql.Replace(name, string.Join(",", list));
                        parName.Remove(name);
                        continue;

                    }
                }
                else
                {
                    parName.Remove(name);
                    listParam.Add(new SqlParameter(name, value));
                }
            }
            return ConverSqlByPar(sql, parName);
        }


        public static string ConverSqlByPar(string sql, List<string> parName)
        {
            if (parName.Count == 0)
            {
                return CreateSqlBySqlPar(sql);
            }
            StringBuilder strPars = new StringBuilder(100);
            foreach (var par in parName)
            {
                if (strPars.Length == 0)
                {
                    strPars.Append(string.Format(@"({0}\s+)",par));
                }
                else
                {
                    strPars.Append(string.Format(@"|({0}\s+)", par));
                }
            }

            var mytregex = new Regex(strPars.ToString());

            sql = mytregex.Replace(sql, "@null");


            return CreateSqlBySqlPar(sql);
        }
    }



}
