using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using Esmart.Framework.Configuration;
using System.Reflection;


namespace Esmart.Framework.DB
{
    /// <summary>
    /// 执行Xml配置中的SQL或HQL语句
    /// </summary>
    public class SqlExecuteHelper
    {
        private static int Timeout
        {
            get
            {
                string temp = "120";
                return Convert.ToInt32(string.IsNullOrWhiteSpace(temp) ? "120" : temp);
            }
        }

        /// <summary>
        /// 执行Xml中的SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="xmlFileName">Xml文件名</param>
        /// <param name="sqlName">Sql节点的Name属性值</param>
        /// <param name="parameters">SQL/HQL语句所需的参数</param>
        /// <returns>执行SQL/HQL语句后，受影响的行数</returns>
        public static int ExecuteNonQuery(string xmlFileName, string sqlName, params QueryParameter[] parameters)
        {
            string[] where;
            string orderby, distinct;
            //parameters = RemoveWhereOrderDistinct<object>(parameters, out where, out orderby, out distinct);
            return 0;
           
        }

        public static string FindXMLString<T>(string xmlFileName, string sqlName, T obj) where T : class
        {
            var sqlEntity = SqlXmlHelper.Instance.FindSQL(xmlFileName, sqlName);
            string sql = sqlEntity.Content; 
            var type = obj.GetType();
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo i in ps)
            {

                object objprop = i.GetValue(obj, null);
                    string name = i.Name;
                    string value = "";
                    if (objprop != null)
                    {
                        value = objprop.ToString();
                    }
                    sql = sql.Replace("@" + i.Name, value.ToString());
            }

            return sql;
        }

        public static string FindSQLString(string xmlFileName, string sqlName, QueryParameter[] parameters, out  List<SqlParameter> lparm)
        {
            var sqlEntity = SqlXmlHelper.Instance.FindSQL(xmlFileName, sqlName);

            string[] where;
            string orderby;
            parameters = RemoveWhereOrderDistinct(parameters, out where, out orderby);
            string sql = "";
           sql= GetExecuteSQL(sqlEntity, where, orderby);
           List<SqlParameter> parmlist = new List<SqlParameter>();
                  foreach (var param in parameters)
                  {
                      parmlist.Add(new SqlParameter(param.Name, param.Value));
                  }
                  lparm = parmlist;
            return sql;
        }

        private static string GetExecuteSQL(SqlEntity entity, string[] where = null, string orderby = null)
        {
            string sql = entity.Content;
        
            if (where != null)
            {
                for (int i = 0; i < where.Length; i++)
                {
                    sql = sql.Replace(string.Format("@{0}where", (i == 0 ? "" : i.ToString())), where[i]);
                }
            }
            if (orderby != null)
            {
                sql = sql.Replace("@orderby", orderby);
            }
            return sql;
        }

     

        private static QueryParameter[] RemoveWhereOrderDistinct(QueryParameter[] parameters, out string[] where, out string order)
        {
            var whereFlag = 0;
            var whereList = new List<string>();
            var orderPara = parameters.FirstOrDefault(p => p.Name == "@orderby");
            order = null;
            var paras = parameters.ToList();
            QueryParameter wherePara = null;
            do
            {
                if (wherePara != null)
                {
                    paras.Remove(wherePara);
                    whereList.Add(wherePara.Value.ToString());
                }
                wherePara = parameters.FirstOrDefault(p => p.Name == string.Format("@{0}where", (whereFlag == 0 ? "" : whereFlag.ToString())));
                whereFlag++;
            } while (wherePara != null);
            where = whereList.ToArray();

            if (orderPara != null)
            {
                paras.Remove(orderPara);
                order = orderPara.Value.ToString();
            }
            parameters = paras.ToArray();
            return parameters;
        }
    }
}
