using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Esmart.Framework.DB.Dapper
{
    public class DBUtils
    {
        private static ConcurrentDictionary<DataBaseType, IDBHelper> _iDBHelpers = new ConcurrentDictionary<DataBaseType, IDBHelper>();



        public static IDbConnection CreateDBConnection(string strKey, out DataBaseType dbType)
        {
            dbType = DataBaseType.SqlServer;
            IDbConnection connection = null;
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[strKey];
            string strConn = connectionStringSettings.ConnectionString;
            string providerName = connectionStringSettings.ProviderName;

            if (string.IsNullOrEmpty(providerName))
            {
                throw new Exception(strKey + "连接字符串未定义 ProviderName");
            }
            else if (providerName == "System.Data.SqlClient")
            {
                dbType = DataBaseType.SqlServer;
                connection = new System.Data.SqlClient.SqlConnection(strConn);
            }
            //else if (providerName == "Oracle.DataAccess.Client" )
            //{
            //    dbType = dbType.Oracle;
            //    connection = new Oracle.DataAccess.Client.OracleConnection(strConn);
            //}
            //else if (providerName == "System.Data.OracleClient")
            //{
            //    dbType = dbType.Oracle;
            //    connection = new System.Data.OracleClient.OracleConnection(strConn);
            //}
            else if (providerName == "MySql.Data.MySqlClient")
            {
                dbType = DataBaseType.MySql;
                connection = new MySql.Data.MySqlClient.MySqlConnection(strConn);
            }
            else if (providerName == "System.Data.OleDb")
            {
                dbType = DataBaseType.Aceess;
                connection = new System.Data.OleDb.OleDbConnection(strConn);
            }
            else
            {
                throw new Exception(strKey + "连接字符串未识别 ProviderName");
            }
            return connection;
        }




        public static IDbConnection CreateDBConnection(DataBaseType dbType, string strKey)
        {
            IDbConnection connection = null;
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[strKey];
            string strConn = connectionStringSettings.ConnectionString;

            switch (dbType)
            {
                case DataBaseType.SqlServer:
                    connection = new System.Data.SqlClient.SqlConnection(strConn);
                    break;
                case DataBaseType.MySql:
                    connection = new MySql.Data.MySqlClient.MySqlConnection(strConn);
                    break;
                //case dbType.Oracle:
                //connection = new Oracle.DataAccess.Client.OracleConnection(strConn);
                //connection = new System.Data.OracleClient.OracleConnection(strConn);
                //break;
                case DataBaseType.Aceess:
                    connection = new System.Data.OleDb.OleDbConnection(strConn);
                    break;
            }
            return connection;
        }


        public static DataBaseType GetDBTypeByConnKey(string connKey)
        {
            DataBaseType dbType = DataBaseType.SqlServer;
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connKey];
            string strConn = connectionStringSettings.ConnectionString;
            string providerName = connectionStringSettings.ProviderName;
            if (string.IsNullOrEmpty(providerName))
            {
                throw new Exception(connKey + "连接字符串未定义 ProviderName");
            }
            else if (providerName == "System.Data.SqlClient")
            {
                dbType = DataBaseType.SqlServer;
            }
            //else if (providerName == "Oracle.DataAccess.Client" )
            //{
            //    dbType = dbType.Oracle;
            //    connection = new Oracle.DataAccess.Client.OracleConnection(strConn);
            //}
            //else if (providerName == "System.Data.OracleClient")
            //{
            //    dbType = dbType.Oracle;
            //    connection = new System.Data.OracleClient.OracleConnection(strConn);
            //}
            else if (providerName == "MySql.Data.MySqlClient")
            {
                dbType = DataBaseType.MySql;
            }
            else if (providerName == "System.Data.OleDb")
            {
                dbType = DataBaseType.Aceess;
            }
            else
            {
                throw new Exception(connKey + "连接字符串未识别 ProviderName");
            }
            return dbType;
        }


        public static IDBHelper GetDBHelper(DataBaseType dbType)
        {
            IDBHelper dbHelper;
            if (!_iDBHelpers.TryGetValue(dbType, out dbHelper))
            {
                switch (dbType)
                {
                    case DataBaseType.SqlServer:
                       // dbHelper = new HY.DataAccess.SqlDBHelper.DBAdaptor();
                        break;
                    //case DataBaseType.MySql:
                    //    dbHelper = new HY.DataAccess.MySqlDBHelper.MySqlAdaptor();
                    //    break;
                    //case DataBaseType.Oracle:
                    //dbHelper = new HY.DataAccess.OracleDBHelper.OracleAdaptor();
                    //break;
                    default:
                       // dbHelper = new HY.DataAccess.SqlDBHelper.DBAdaptor();
                        break;
                }
                _iDBHelpers[dbType] = dbHelper;
            }
            return dbHelper;
        }



        public static List<IDataParameter> ConvertToDbParameter(Dictionary<string, Parameter> parmList, DataBaseType dbType)
        {
            List<IDataParameter> dbParamList = new List<IDataParameter>();
            foreach (var item in parmList)
            {
                dbParamList.Add(ConvertToIDataParameter(item.Value.ParameterName, item.Value.ParameterValue, dbType));
            }
            return dbParamList;
        }


        public static IDataParameter ConvertToIDataParameter(string parameterName, object value, DataBaseType dbType)
        {

            switch (dbType)
            {
                case DataBaseType.SqlServer:
                    return new System.Data.SqlClient.SqlParameter(parameterName, value);
                case DataBaseType.MySql:
                    return new MySql.Data.MySqlClient.MySqlParameter(parameterName, value);
                case DataBaseType.Oracle:
                    return new System.Data.OleDb.OleDbParameter(parameterName, value);
                default:
                    return new System.Data.SqlClient.SqlParameter(parameterName, value);
            }
        }



        public static DbConnObj GetConnObj(IDbConnection dbConnection, IDbTransaction transaction = null)
        { 
            DbConnObj dbConnObj = new DbConnObj(); 
            if (null != transaction)
            {
                dbConnObj.DbConnection = transaction.Connection;
                dbConnObj.DbTransaction = transaction;
            }
            else if (null != dbConnection)
            {
                dbConnObj.DbConnection = dbConnection;                
            }
            return dbConnObj;
        }

    }

    public class DbConnObj
    {
        public IDbTransaction DbTransaction { get; set; }

        public IDbConnection DbConnection { get; set; }

        public DataBaseType dbType { get; set; }
    }
}