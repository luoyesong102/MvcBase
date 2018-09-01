
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using Esmart.Framework.Caching;
using Esmart.Framework.Model;
using System.Data;
using System.Reflection;
using Esmart.Framework.Logging;
using System.Text.RegularExpressions;

namespace Esmart.Framework.DB
{
    public partial class TSqlExec : IDbExec
    {

        public int ExecuteSql(string sql, DbParameter[] para = null)
        {

            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);

                if (para != null && para.Count() > 0)
                {
                    cmd.Parameters.AddRange(para);
                }
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
                string key = sql;
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                if (para == null)
                {
                    para = new DbParameter[0];
                }
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
                Clear();
            }
            return rows;
        }

        public T ExecuteProcedure<T>(string procedureName, object where)
        {
            var type = typeof(T);

            SqlConnection connection = null;
            DbCommand cmd = null;
            using (connection = new SqlConnection(ConnectDalString))
            {
                connection.Open();
                using (cmd = new SqlCommand(procedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var data = CreateSQl.GetWhere(where);

                    cmd.Parameters.AddRange(data.ToArray());

                    var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    if (type.IsValueType || type == typeof(string))
                    {
                        using (reader)
                        {
                            if (reader.Read())
                            {
                                int ilength = reader.FieldCount;
                                if (ilength == 0)
                                {
                                    return default(T);
                                }
                                return (T)Convert.ChangeType(reader.GetValue(0), type);
                            }
                        }

                    }
                    if (type.IsClass)
                    {
                        return ConverToAllModel<T>(reader).FirstOrDefault();
                    }

                }
            }
            return default(T);
        }

        public int InsertObject<Tobj>(Tobj obj)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listP = new List<DbParameter>();
            string sql = CreateSQl.InsertObjectString(obj, ref listP);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);

                connection.Open();
                cmd = new SqlCommand(sql, connection);

                cmd.Parameters.AddRange(listP.ToArray());

                object returnobj = cmd.ExecuteScalar();

                if (returnobj != null && returnobj != DBNull.Value)
                {
                    rows = Convert.ToInt32(returnobj);
                }

            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listP));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            return rows;
        }


        public void InsertObjectAsy<Tobj>(Tobj obj)
        {
            Type type = typeof(Tobj);

            List<DbParameter> listP = new List<DbParameter>();
            string sql = CreateSQl.InsertObjectString(obj, ref listP);

            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddRange(listP.ToArray());
                cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        public List<int> InsertListObject<Tobj>(IEnumerable<Tobj> obj)
        {
            List<int> returnList = new List<int>();
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listP = new List<DbParameter>();
            string sql = CreateSQl.InsertListObjectString(obj, ref listP);

            var reader = ExecuteReader(sql, listP.ToArray());

            using (reader)
            {
                while (reader.Read())
                {
                    var value = reader.GetValue(0);

                    if (value != null && value != DBNull.Value)
                    {
                        int returnCount = 0;
                        int.TryParse(value.ToString(), out returnCount);
                        if (returnCount > 1)
                        {
                            returnList.Add(returnCount);
                        }
                    }

                    if (!reader.NextResult())
                    {
                        break;
                    }
                }
            }
            return returnList;
        }

        public int UpdateObject<Tobj>(Tobj obj)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listp = new List<DbParameter>();
            string sql = CreateSQl.UpdateObjectString(obj, ref listp);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddRange(listp.ToArray());
                rows = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listp));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            return rows;
        }

        public int UpdateListObject<Tobj>(IEnumerable<Tobj> obj)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listp = new List<DbParameter>();
            string sql = CreateSQl.UpdateListObjectString(obj, ref listp);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddRange(listp.ToArray());
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listp));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }

            }
            return rows;
        }



        public int UpdateObjectAllField<Tobj>(Tobj obj)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listp = new List<DbParameter>();
            string sql = CreateSQl.UpdateObjectString(obj, ref listp, false);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddRange(listp.ToArray());
                rows = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listp));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            return rows;
        }

        public int UpdateListObjectAllField<Tobj>(IEnumerable<Tobj> obj)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listp = new List<DbParameter>();
            string sql = CreateSQl.UpdateListObjectString(obj, ref listp, false);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddRange(listp.ToArray());
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listp));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            return rows;
        }

        public int DeleteObj<Tobj>(object id)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listp = new List<DbParameter>();
            string sql = CreateSQl.DeleteObjectString<Tobj>(id, ref listp);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);
                connection.Open();
                cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddRange(listp.ToArray());
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listp));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            return rows;
        }

        public int DeleteListObj<Tobj>(IEnumerable<object> ids)
        {
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateCache().Remove(type.FullName);
            }
            List<DbParameter> listp = new List<DbParameter>();
            string sql = CreateSQl.DeleteListObjectString<Tobj>(ids, ref listp);
            int rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectDalString);

                connection.Open();
                cmd = new SqlCommand(sql, connection);

                cmd.Parameters.AddRange(listp.ToArray());
                rows = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(sql, listp));
                throw ex;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            return rows;
        }


        public Tobj Find<Tobj>(object model)
        {
            List<DbParameter> list = new List<DbParameter>();

            string sql = CreateSQl.CreateModelString<Tobj>(model, ref list);

            return ConverToAllModel<Tobj>(ExecuteReader(sql, list.ToArray())).FirstOrDefault();
        }

        public List<Tobj> FindAll<Tobj>()
        {

            List<Tobj> list = null;
            string sql = CreateSQl.CreateListString<Tobj>();
            Type type = typeof(Tobj);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                list = CacheManager.CreateCache().Get<List<Tobj>>(type.FullName);
            }
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                list = new List<Tobj>();
            }
            IDataReader reader = ExecuteReader(sql, null);
            list = ConverToAllModel<Tobj>(reader);
            if (attrs.Length != 0)
            {
                CacheAttribute cacheAttri = ((CacheAttribute)attrs[0]);
                if (cacheAttri.ExpiresAt == 0)
                {
                    CacheManager.CreateCache().Set<List<Tobj>>(type.FullName, list);
                }
                else
                {
                    CacheManager.CreateCache().Set<List<Tobj>>(type.FullName, list, DateTime.Now.AddMinutes(cacheAttri.ExpiresAt));
                }
            }
            return list;
        }



        public List<Tobj> FindAll<Tobj>(object model)
        {

            List<Tobj> list = new List<Tobj>();

            List<DbParameter> listP = new List<DbParameter>();

            string sql = CreateSQl.CreateListString<Tobj>(model, ref listP);

            Type type = typeof(Tobj);

            IDataReader reader = ExecuteReader(sql, listP.ToArray());

            list = ConverToAllModel<Tobj>(reader);

            return list;
        }




        public List<Tobj> FindListByWhere<Tobj>(string where, System.Data.Common.DbParameter[] para)
        {


            string sql = CreateSQl.CreateListStringByWhere<Tobj>(where);
            IDataReader reader = ExecuteReader(sql, para);
            List<Tobj> list = ConverToAllModel<Tobj>(reader);
            return list;
        }

        public List<Tobj> FindList<Tobj>(Model.DBPage page, out int count)
        {

            List<DbParameter> listP = new List<DbParameter>();

            string sql = CreateSQl.CreateListString<Tobj>(page.Where, ref listP);

            page.Where = null;

            return FindListBySql<Tobj>(sql, page, out count, listP.ToArray());
        }


        public List<Tobj> FindList<Tobj>(string strSQL, DbParameter[] para = null)
        {
            IDataReader reader = ExecuteReader(strSQL, para);
            List<Tobj> list = ConverToAllModel<Tobj>(reader);
            return list;
        }


        public IDataReader ExecuteReader(string strSQL, DbParameter[] para = null)
        {
            SqlConnection connection = null;

            SqlCommand cmd = null;

            try
            {
                connection = new SqlConnection(ConnectSelectString);
                connection.Open();
                cmd = new SqlCommand(strSQL, connection);
                if (para != null && para.Count() > 0)
                {
                    cmd.Parameters.AddRange(para);
                }
                IDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;

            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, ex);
                if (para == null)
                {
                    para = new DbParameter[0];
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
                string sql = this.Print(strSQL, para.ToList());
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, sql);

                Exception exception = new Exception(ex.Message + ",sql语句是:" + sql);
                throw exception;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                Clear();
            }
        }


        public object ExecuteScalar(string strSQL, DbParameter[] para = null)
        {
            object rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
                connection = new SqlConnection(ConnectSelectString);

                connection.Open();

                cmd = new SqlCommand(strSQL, connection);

                if (para != null && para.Count() > 0)
                {
                    cmd.Parameters.AddRange(para);
                }
                return cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, e);
                if (para == null)
                {
                    para = new DbParameter[0];
                }
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Warnning(key, this.Print(strSQL, para.ToList()));
                throw e;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
                Clear();

            }

        }


        public string CreateSqlByParSql(string sql, object model, ref List<DbParameter> dbPar)
        {

            return CreateSQl.CreateSqlBySqlPar(sql, model, ref dbPar);
        }

        public string ConnectKey
        {
            get;
            set;
        }


        protected string ConnectSelectString
        {
            get
            {
                var connect = ConfigurationManager.ConnectionStrings[ConnectKey + "_Select"];
                if (connect != null)
                {
                    return connect.ConnectionString;
                }
                return ConfigurationManager.ConnectionStrings[ConnectKey].ConnectionString;
            }
        }

        protected string ConnectDalString
        {
            get
            {
                var connect = ConfigurationManager.ConnectionStrings[ConnectKey + "_Dal"];

                if (connect != null)
                {
                    return connect.ConnectionString;
                }

                return ConfigurationManager.ConnectionStrings[ConnectKey].ConnectionString;
            }
        }


        public List<TReturn> FindListByName<TReturn>(string name, object tmodel, Func<string, string> injectionSql = null, bool userSqlContent = false)
        {

            var model = CommonFunction.GetSqlStringByName(name);

            if (model == null)
            {
                throw new Exception("sql语句没找到");
            }

            List<TReturn> listReturn = new List<TReturn>();

            Type type = typeof(TReturn);

            List<DbParameter> list = new List<DbParameter>();

            string sql = CreateSQl.CreateDbParameter(tmodel, model, ref list, userSqlContent);

            if (injectionSql != null)
            {
                sql = injectionSql(sql);
            }

            using (new Trace(name))
            {
                IDataReader reader = ExecuteReader(sql, list.ToArray());
                return ConverToAllModel<TReturn>(reader);
            }
        }


        public List<TReturn> FindListByName<TReturn>(string name, DBPage where, out int count, Func<string, string> injectionSql = null, bool userSqlContent = false)
        {

            List<TReturn> listTReturn = new List<TReturn>();

            var model = CommonFunction.GetSqlStringByName(name);

            if (model == null)
            {
                throw new Exception("sql语句没找到");
            }

            List<TReturn> listReturn = new List<TReturn>();

            Type type = typeof(TReturn);

            List<DbParameter> list = new List<DbParameter>();

            string sql = CreateSQl.CreateDbParameter(where.Where, model, ref list, userSqlContent);

            if (injectionSql != null)
            {
                sql = injectionSql(sql);
            }
            where.Where = null;

            return FindListBySql<TReturn>(sql, where, out count, list.ToArray());
        }


        protected List<Tobj> ConverToAllModel<Tobj>(IDataReader reader)
        {
            Type type = typeof(Tobj);

            var objs = GetColNameAttribute(type);

            using (reader)
            {
                List<Tobj> list = new List<Tobj>();

                int ilength = reader.FieldCount;

                while (reader.Read())
                {
                    Tobj obj = ConverToModel<Tobj>(reader, type, objs);

                    list.Add(obj);
                }
                return list;
            }
        }


        protected Tobj ConverToModel<Tobj>(IDataReader reader, Type type, Dictionary<string, PropertyInfo> allPro)
        {
            int ilength = reader.FieldCount;
            Tobj obj = System.Activator.CreateInstance<Tobj>();
            for (int i = 0; i < ilength; i++)
            {
                object objValue = reader.GetValue(i);
                if (objValue != null && objValue != DBNull.Value)
                {
                    string dbName = reader.GetName(i);

                    PropertyInfo property = null;

                    var isfind = allPro.TryGetValue(dbName.ToLower(), out property);

                    if (isfind && property != null)
                    {
                        if (property.PropertyType.IsGenericType)
                        {
                            var types = property.PropertyType.GetGenericArguments();

                            if (types.Count() > 0 && types[0].IsEnum)
                            {
                                var name = Enum.GetName(types[0], objValue);

                                var data = Enum.Parse(types[0], name);

                                property.SetValue(obj, data, null);
                            }
                            else if (types.Count() > 0)
                            {

                                var objnewValue = Convert.ChangeType(objValue, types[0]);

                                property.SetValue(obj, objnewValue, null);
                            }
                        }
                        else
                        {
                            property.SetValue(obj, objValue, null);
                        }

                    }
                }
            }
            return obj;
        }


        private Dictionary<string, PropertyInfo> GetColNameAttribute(Type type)
        {

            var dic = new Dictionary<string, PropertyInfo>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertie in properties)
            {
                var objs = propertie.GetCustomAttributes(typeof(ColNameAttribute), false) as ColNameAttribute[];

                if (objs.Length > 0)
                {
                    if (!dic.ContainsKey(objs[0].Name))
                    {
                        dic.Add(objs[0].Name.ToLower(), propertie);
                    }
                }

                if (!dic.ContainsKey(propertie.Name))
                {
                    dic.Add(propertie.Name.ToLower(), propertie);
                }
            }
            return dic;
        }


        public List<TReturn> FindListBySql<TReturn>(string sql, DBPage tmodel, out  int count, DbParameter[] dbPar = null)
        {
            List<TReturn> list = new List<TReturn>();

            if (tmodel.Where != null)
            {
                if (tmodel.Where.GetType() == typeof(string))
                {
                    sql = string.Format(" {0} {1}", sql, tmodel.Where);
                }
                else
                {
                    List<DbParameter> listDb = new List<DbParameter>();

                    CreateSQl.CreateSqlBySqlPar(sql, tmodel.Where, ref listDb);

                    dbPar = listDb.ToArray();
                }
            }

            if (!string.IsNullOrEmpty(tmodel.OrderBy))
            {
                Regex myregex = new Regex(@"order\s+by\s+");
                if (!myregex.IsMatch(sql.ToLower()))
                {

                    sql = string.Format("{0} order by {1} {2}", sql, tmodel.OrderBy, tmodel.SortCol);
                }
            }
            int index = 0;
            if (tmodel.PageIndex <= 1)
            {
                tmodel.PageIndex = 1;
            }
            if (tmodel.PageSize <= 0)
            {
                tmodel.PageSize = 20;
            }
            var currentFirst = (tmodel.PageIndex - 1) * tmodel.PageSize;
            var currentlast = tmodel.PageIndex * tmodel.PageSize;

            using (new Trace(typeof(DBPage).FullName))
            {
                Type type = typeof(TReturn);

                var allpro = GetColNameAttribute(type);

                using (IDataReader reader = ExecuteReader(sql, dbPar))
                {
                    while (reader.Read())
                    {
                        if (index >= currentFirst && index < currentlast)
                        {
                            list.Add(ConverToModel<TReturn>(reader, type, allpro));
                        }
                        index++;
                    }
                }
                count = index;
            }
            return list;
        }


        public Tobj Find<Tobj>(System.Linq.Expressions.Expression<Func<Tobj, bool>> express) where Tobj : class
        {
            var fun = express.Compile();

            return FindAll<Tobj>().FirstOrDefault(fun);
        }
        public List<Tobj> FindAll<Tobj>(System.Linq.Expressions.Expression<Func<Tobj, bool>> express) where Tobj : class
        {
            var fun = express.Compile();

            return this.FindAll<Tobj>().Where(fun).ToList();
        }

        public ITransactionDbExec CreateTransaction()
        {
            return this;
        }


    }


    public partial class TSqlExec : IDbExec
    {


        internal TSqlExec()
        {

        }

        StringBuilder StrSql = new StringBuilder(400);

        List<DbParameter> list = new List<DbParameter>();

        bool isUserBracket = false;


        public ISmartDbExec Query(object model)
        {
            StrSql.Append(CreateSQl.Query(model, ref list));

            return this;
        }


        public ISmartDbExec AndLeftBrackets()
        {
            StrSql.Append(" and (");
            isUserBracket = true;
            return this;
        }

        public ISmartDbExec OrLeftBrackets()
        {
            StrSql.Append(" or (");

            isUserBracket = true;

            return this;
        }

        public ISmartDbExec RightBrackets()
        {
            StrSql.Append(")");

            Regex myregex = new Regex(@"\({1}\s*\){1}$");

            string sql = myregex.Replace(StrSql.ToString(), "(1=1)");

            StrSql = new StringBuilder(400);

            StrSql.Append(sql);

            return this;
        }


        public ISmartDbExec AndQuery(object obj)
        {
            StrSql.Append(CreateSQl.Query(obj, ref list, "and"));

            return this;
        }

        public ISmartDbExec OrQuery(object obj)
        {
            StrSql.Append(CreateSQl.Query(obj, ref list, "or"));

            return this;
        }

        public ISmartDbExec OrLessThanQuery(object obj)
        {
            StrSql.Append(CreateSQl.LessQuery(obj, ref list, "or"));

            return this;
        }

        public ISmartDbExec AndLessThanQuery(object obj)
        {
            StrSql.Append(CreateSQl.LessQuery(obj, ref list, "and"));

            return this;
        }

        public ISmartDbExec OrGreaterThanQuery(object obj)
        {
            StrSql.Append(CreateSQl.GreateQuery(obj, ref list, "or"));

            return this;
        }

        public ISmartDbExec AndGreaterThanQuery(object obj)
        {
            StrSql.Append(CreateSQl.GreateQuery(obj, ref list, "and"));

            return this;
        }

        public ISmartDbExec AndFuzzyQuery(object obj)
        {
            StrSql.Append(CreateSQl.FuzzyQuery(obj,ref list, "and"));

            return this;
        }

        public ISmartDbExec OrFuzzyQuery(object obj)
        {
            StrSql.Append(CreateSQl.FuzzyQuery(obj, ref list, "or"));

            return this;
        }

        public ISmartDbExec AndQueryIn(object obj)
        {
            StrSql.Append(CreateSQl.InQuery(obj, "and",ref list));

            return this;
        }

        public ISmartDbExec OrQueryIn(object obj)
        {
            StrSql.Append(CreateSQl.InQuery(obj, "or",ref list));

            return this;
        }

        public ISmartDbExec AndInTableQuery<Tobj>(string mainId, object obj, string tableId = null)
        {
            StrSql.Append(CreateSQl.InTableQuery<Tobj>("and", mainId, obj, ref list, tableId));

            return this;
        }

        public ISmartDbExec OrInTableQuery<Tobj>(string mainId, object obj, string tableId = null)
        {
            StrSql.Append(CreateSQl.InTableQuery<Tobj>("or", mainId, obj, ref list, tableId));

            return this;
        }

        public List<T> ToList<T>()
        {
            string sql = null;

            if (this.isUserBracket)
            {
                Regex myregex = new Regex(@"\({1}\s*(and|or)");

                string copysql = myregex.Replace(StrSql.ToString(), "(");

                StrSql = new StringBuilder(400);

                StrSql.Append(copysql);
            }

            Type type = typeof(T);

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                sql = string.Format(" select  *  from {0} where  IsDelete=0 {1}", CreateSQl.GetNameByType(type) + "(nolock)", StrSql.ToString());
            }
            else
            {
                sql = string.Format(" select  *  from {0} where  1=1 {1}", CreateSQl.GetNameByType(type) + "(nolock)", StrSql.ToString());
            }

            var reader = this.ExecuteReader(sql, list.ToArray());
          
            return ConverToAllModel<T>(reader);
        }

        public int Delete<T>()
        {
            ResponseHeader code = new ResponseHeader();

            string sql = null;

            Type type = typeof(T);
            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateRedisCache().Remove(type.FullName);
            }

            if (StrSql.Length == 0)
            {
                throw new TpoBaseException("没有删除条件，请按照条件删除");
            }
            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                sql = string.Format(" UPDATE {0}  SET IsDelete=1  where 1=1   {1}", CreateSQl.GetNameByType(type), StrSql.ToString());
            }
            else
            {
                sql = string.Format(" DELETE {0}  where 1=1  {1}", CreateSQl.GetNameByType(type), StrSql.ToString());
            }

            return this.ExecuteSql(sql, list.ToArray());
        }

        public ResponseHeader Update<T>(T obj)
        {
            ResponseHeader code = new ResponseHeader();

            string message = null;

            if (StrSql.Length == 0)
            {
                throw new TpoBaseException("没有修改条件，请按照条件修改");
            }

            if (!CommonFunction.ValidAll(obj, ref message))
            {
                code.ReturnCode = -1;
                code.Message = message;
                return code;
            }
            string sql = CreateSQl.UpdateObjectBySmart(obj, ref list);

            Type type = typeof(T);

            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateRedisCache().Remove(type.FullName);
            }

            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                sql += string.Format("  where  IsDelete=0 {0}", StrSql.ToString());
            }
            else
            {
                sql += string.Format("  where  1=1 {0}", StrSql.ToString());
            }
            code.ReturnCode = this.ExecuteSql(sql, list.ToArray());
            Clear();
            return code;
        }

        public string Print()
        {
            return Print(StrSql.ToString(), list);
        }

        public ResponseHeader Add<T>(T model)
        {
            ResponseHeader code = new ResponseHeader();

            string message = null;

            var returnCode = CommonFunction.ValidAll(model, ref message);

            if (!returnCode)
            {
                code.ReturnCode = -1;
                code.Message = message;
                return code;
            }

            Type type = typeof(T);

            var attrs = type.GetCustomAttributes(typeof(CacheAttribute), false);
            if (attrs.Length != 0)
            {
                CacheManager.CreateRedisCache().Remove(type.FullName);
            }
            code.ReturnCode = this.InsertObject(model);
            Clear();
            return code;
        }

        internal string Print(string sql, List<DbParameter> listDbP)
        {
            string newstr = sql;

            if (this.isUserBracket)
            {
                Regex myregex = new Regex(@"\({1}\s*(and|or)");

                newstr = myregex.Replace(StrSql.ToString(), "(");
            }
            foreach (var model in listDbP)
            {
                if (model.Value == null)
                {
                    continue;
                }
                if ((model.Value.GetType().IsClass && model.Value.GetType() == typeof(string)) || model.Value.GetType() == typeof(DateTime))
                {
                    newstr = newstr.Replace(model.ParameterName, "'" + model.Value.ToString().Trim() + "'");
                }
                else
                {
                    newstr = newstr.Replace(model.ParameterName, model.Value.ToString());
                }

            }
            return newstr;
        }

        public bool Exist<T>()
        {
            Type type = typeof(T);
            string sql = null;
            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                sql = string.Format("SELECT  count(1)  from {0} where IsDelete=0 {2}", CreateSQl.GetNameByType(type) + "(nolock)", StrSql.ToString());
            }
            else
            {
                sql = string.Format("SELECT  count(1)  from {0} where 1=1 {2}", CreateSQl.GetNameByType(type) + "(nolock)", StrSql.ToString());
            }

            var obj = this.ExecuteScalar(sql, list.ToArray());
 
            if (obj != null && obj != DBNull.Value)
            {
                return Convert.ToInt32(obj) > 0;
            }
            return false;
        }

        public int Count<T>()
        {
            Type type = typeof(T);
            string sql = null;
            var property = type.GetProperty(CreateSQl.IsDelete, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                sql = string.Format("SELECT  count(1)  from {0} where IsDelete=0 {2}", CreateSQl.GetNameByType(type) + "(nolock)", StrSql.ToString());
            }
            else
            {
                sql = string.Format("SELECT  count(1)  from {0} where 1=1 {2}", CreateSQl.GetNameByType(type) + "(nolock)", StrSql.ToString());
            }

            var obj = this.ExecuteScalar(sql, list.ToArray());

            if (obj != null && obj != DBNull.Value)
            {
                return Convert.ToInt32(obj);
            }
            return 0;
        }


        public override string ToString()
        {
            string sql = this.Print();
            this.Clear();
            return sql;
        }

        internal List<DbParameter> Pars
        {
            get { return list; }
        }


        private void Clear()
        {
            StrSql.Clear();
            list.Clear();
        }
    }


    public partial class TSqlExec : ITransactionDbExec
    {

        List<DbParameter> pars = new List<DbParameter>(100);

        List<string> sqls = new List<string>(5);
        public int Commit()
        {
            if (sqls.Count == 0)
            {
                return 0;
            }
            string commitSql = "begin tran {0} if @@error<>0 begin  rollback tran end else  begin commit tran end";
            StringBuilder strSqls = new StringBuilder(300);

            foreach (var sql in sqls)
            {
                strSqls.Append(sql);
            }
            commitSql = string.Format(commitSql, strSqls.ToString());

            int code = this.ExecuteSql(commitSql, pars.ToArray());

            pars.Clear();
            sqls.Clear();

            return code;
        }

        public ITransactionDbExec TransactionAdd<T>(T model) where T:class
        {
            string sql = CreateSQl.InsertObjectString(model, ref pars);
            sqls.Add(sql);
            return this;
        }

        public ITransactionDbExec TransactionUpdate<T>(T model, ISmartDbExec exec = null) where T : class
        {
            if (exec != null)
            {
                if (this.list.Count == 0)
                {
                    throw new TpoBaseException("修改条件不能为空，请输入修改条件");
                }
                this.pars.AddRange(this.list);
                sqls.Add(string.Format("{0} where 1=1  {1}", CreateSQl.UpdateObjectString(model, ref  pars, true, false), exec.ToString()));
            }
            else
            {
                sqls.Add(CreateSQl.UpdateObjectString(model, ref  pars));
            }
            return this;
        }


        public ITransactionDbExec TransactionDelete<T>(object Id = null, ISmartDbExec exec = null) where T : class
        {
            if (exec != null)
            {
                if (this.list.Count == 0)
                {
                    throw new TpoBaseException("删除条件不能为空，请输入删除条件");
                }
                this.pars.AddRange(this.list);
                sqls.Add(string.Format("{0} where 1=1  {1}", CreateSQl.DeleteObjectString<T>(), exec.ToString()));
            }
            else
            {
                if (Id == null)
                {
                    throw new TpoBaseException("唯一key不能为空");
                }
                sqls.Add(CreateSQl.DeleteObjectString<T>(Id, ref pars));
            }
            return this;
        }

        public void Dispose()
        {
            Commit();
        }
    }


    public partial class TSqlSelect : IDbSelect
    {

        DbConnection connect = null;


        public TSqlSelect(DbConnection _connect)
        {
            connect = _connect;
        }



        public SoaDataPageResponse<T> FindList<T>(string sql, SoaDataPage page, DbParameter[] dbPar = null) where T : new()
        {
            SoaDataPageResponse<T> response = new SoaDataPageResponse<T>();


            if (_ignoreCase)
            {
                sql = Regex.Replace(sql, @"select[\s\S]*from", "select count(1) from", RegexOptions.IgnoreCase);
            }
            else
            {
                sql = Regex.Replace(sql, @"select[\s\S]*from", "select count(1) from");
            }

            response.Count = ExecuteScalar(sql, dbPar);

            if (!string.IsNullOrEmpty(page.OrderBy))
            {
                sql = string.Format("{0}  ORDER BY {1} {2}", sql, page.OrderBy, page.SortCol);
            }

            if (page.PageIndex <= 0)
            {
                page.PageIndex = 1;
            }
            if (page.PageSize <= 0)
            {
                page.PageSize = 10;
            }
            sql = string.Format("{0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", sql, (page.PageIndex - 1) * page.PageSize, page.PageIndex * page.PageSize);

            response.Body = FindList<T>(sql, dbPar);

            return response;
        }

        public List<T> FindList<T>(string sql, DbParameter[] dbPar = null) where T : new()
        {
            var reader = ExecuteReader(sql, dbPar);

            return ConverToAllModel<T>(reader);
        }



        private IDataReader ExecuteReader(string strSQL, DbParameter[] para = null)
        {
            SqlConnection connection = null;

            SqlCommand cmd = null;

            try
            {
                connection.Open();
                cmd = new SqlCommand(strSQL, connection);
                if (para != null && para.Count() > 0)
                {
                    cmd.Parameters.AddRange(para);
                }
                IDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;

            }
            catch (Exception ex)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }

                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(strSQL, ex);

                Exception exception = new Exception(ex.Message + ",sql语句是:" + strSQL);
                throw exception;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }

            }
        }



        private Tobj ConverToModel<Tobj>(IDataReader reader, Dictionary<string, PropertyInfo> allPro) where Tobj : new()
        {
            int ilength = reader.FieldCount;
            Tobj obj = new Tobj();
            for (int i = 0; i < ilength; i++)
            {
                object objValue = reader.GetValue(i);
                if (objValue != null && objValue != DBNull.Value)
                {
                    string dbName = reader.GetName(i);

                    PropertyInfo property = null;

                    var isfind = allPro.TryGetValue(dbName, out property);

                    if (isfind && property != null)
                    {
                        if (property.PropertyType.IsGenericType)
                        {
                            var types = property.PropertyType.GetGenericArguments();

                            if (types.Count() > 0 && types[0].IsEnum)
                            {
                                var name = Enum.GetName(types[0], objValue);

                                var data = Enum.Parse(types[0], name);

                                property.SetValue(obj, data, null);
                            }
                            else if (types.Count() > 0)
                            {

                                var objnewValue = Convert.ChangeType(objValue, types[0]);

                                property.SetValue(obj, objnewValue, null);
                            }
                        }
                        else
                        {
                            property.SetValue(obj, objValue, null);
                        }

                    }
                }
            }
            return obj;
        }


        protected List<Tobj> ConverToAllModel<Tobj>(IDataReader reader) where  Tobj:new()
        {
            Type type = typeof(Tobj);

            var objs = GetColNameAttribute(type);

            using (reader)
            {
                List<Tobj> list = new List<Tobj>();

                int ilength = reader.FieldCount;

                while (reader.Read())
                {
                    Tobj obj = ConverToModel<Tobj>(reader, objs);

                    list.Add(obj);
                }
                return list;
            }
        }


        private Dictionary<string, PropertyInfo> GetColNameAttribute(Type type)
        {

            var dic = new Dictionary<string, PropertyInfo>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertie in properties)
            {
                var objs = propertie.GetCustomAttributes(typeof(ColNameAttribute), false) as ColNameAttribute[];

                if (objs.Length > 0)
                {
                    if (!dic.ContainsKey(objs[0].Name))
                    {
                        dic.Add(objs[0].Name.ToLower(), propertie);
                    }
                }

                if (!dic.ContainsKey(propertie.Name))
                {
                    dic.Add(propertie.Name.ToLower(), propertie);
                }
            }
            return dic;
        }


        private bool _ignoreCase = false;

        public bool IgnoreCase
        {
            get
            {
                return _ignoreCase;
            }
            set
            {
                _ignoreCase = value;
            }
        }


        private int ExecuteScalar(string strSQL, DbParameter[] para = null)
        {
            object rows = 0;
            SqlConnection connection = null;
            DbCommand cmd = null;
            try
            {
               
                connection.Open();

                cmd = new SqlCommand(strSQL, connection);

                if (para != null && para.Count() > 0)
                {
                    cmd.Parameters.AddRange(para);
                }
                var obj= cmd.ExecuteScalar();
                if (obj != null && obj != DBNull.Value)
                {
                    return Convert.ToInt32(obj);
                }
                return 0;

            }
            catch (Exception e)
            {
                string key = "数据库操作失败" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(key, e);
                if (para == null)
                {
                    para = new DbParameter[0];
                }
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Error(strSQL, e);
                throw e;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (connection != null)
                {
                    connection.Dispose();
                }
             
            }

        }
    }
}
