#region using
using System;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;


#endregion

namespace Esmart.Framework.DB
{
    /// <summary>
    /// <table style="font-size:12px">
    /// <tr><td><b>文 件 名</b>：DbObject.cs</td></tr> 
    /// <tr><td><b>功能描述</b>：数据层基类，提供对底层数据的基本操作</td></tr>
    /// <tr><td><b>创 建 人</b>： </td></tr>
    /// <tr><td><b>创建时间</b>：</td></tr>
    /// </table>
    /// </summary>
    public class Oracledbhelp
    {
        #region 成员变量
        /// <summary>
        /// <table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：Oracle数据连接对象</td></tr> 
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table>
        /// </summary>

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：数据连接字符串</td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr> 
        /// </table></summary>
        private static string connectionString;
        #endregion
        private OracleConnection ocon;
        private int timeout = 300000;


        #region 构造函数
        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：构造函数，使用配置文件中的默认数据连接字符串ConnectionString，初始化数据连接对象 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        public Oracledbhelp()
        {
           // connectionString = connstr;//从Web.Config中取得的连接字符串
            ocon = new OracleConnection(connectionString);
        }
        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：构造函数，根据指定的数据连接字符串，初始化数据连接对象</td></tr> 
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        /// <param name="newConnectionString">数据连接字符串</param>
        public Oracledbhelp(string newConnectionString)
        {
            connectionString = newConnectionString;
            ocon = new OracleConnection(connectionString);
        }
        #endregion

        #region 私有方法

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：创建一个OracleCommand对象，用于生成OracleDataReader </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">存储过程的参数对象列表（数组）</param>
        /// <returns>OracleCommand对象</returns>
        private OracleCommand BuildCommand(string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, ocon);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = timeout;

            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;

        }
        #endregion

        #region 运行存储过程
        /// <summary>
        /// <table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行存储过程，获取影响行数，返回存储过程运行结果 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table>
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">存储过程的参数对象列表（数组）</param>
        /// <param name="rowsAffected">出参：执行存储过程所影响的记录行数</param>
        /// <returns>存储过程的运行结果</returns>
        public object RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            object result;

            //if(ocon.State.ToString() == "Closed") Open();
            Open();
            OracleCommand command = BuildCommand(storedProcName, parameters);
            rowsAffected = command.ExecuteNonQuery();
            //如果有"ReturnValue"参数则返回值，否则返回null
            bool blnHasReturn = false;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Direction == ParameterDirection.ReturnValue)
                {
                    blnHasReturn = true;
                    break;
                }
            }
            if (blnHasReturn)
                result = command.Parameters["ReturnValue"].Value;
            else
                result = null;

            Close();
            return result;
        }

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行存储过程，返回产生的OracleDataReader对象,本方法不关闭连接，因此一定要调用语句中手动关闭链接 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">存储过程的参数对象列表（数组）</param>
        /// <returns>OracleDataReader对象</returns>
        public OracleDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            OracleDataReader returnReader;

            Open();
            OracleTransaction myOracleTransaction = ocon.BeginTransaction();
            try
            {
                OracleCommand command = BuildCommand(storedProcName, parameters);
                command.CommandType = CommandType.StoredProcedure;

                returnReader = command.ExecuteReader();
                myOracleTransaction.Commit();

            }
            catch (Exception e)
            {
                myOracleTransaction.Rollback();
                OracleCommand command2 = BuildCommand(storedProcName, parameters);
                command2.CommandType = CommandType.StoredProcedure;

                returnReader = command2.ExecuteReader();
            }
            //Close();
            return returnReader;
        }

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行存储过程，创建一个DataSet对象，
        /// 将运行结果存入指定的DataTable中，返回DataSet对象 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">存储过程的参数对象列表（数组）</param>
        /// <param name="tableName">数据表名称</param>
        /// <returns>DataSet对象</returns>
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            DataSet dataSet = new DataSet();
            OracleDataAdapter sqlDA = new OracleDataAdapter();
            try
            {
                
                Open();
                OracleTransaction myOracleTransaction = ocon.BeginTransaction();

               
                sqlDA.SelectCommand = BuildCommand(storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                myOracleTransaction.Commit();
                Close();
            }
            catch (Exception e)
            {
                try
                {
                    Close();
                  //  connectionString = Constant.GetOracleConn();//从Web.Config中取得的连接字符串
                    ocon = new OracleConnection(connectionString);

                    Open();

                    //OracleDataAdapter sqlDA = new OracleDataAdapter();
                   // sqlDA.SelectCommand = BuildCommand(storedProcName, parameters);
                    sqlDA.Fill(dataSet, tableName);
                    
                   // HGERROR.log(e.ToString() + "OracleTransaction error,cancel transaction ,rebuild sqlDA.Fill! storedProcName:" + storedProcName);
                    Close();
                }
                catch (Exception ex)
                {
                    DataTable dt = new DataTable();
                    dataSet.Tables.Add(dt);
                    //HGERROR.log(ex.ToString() + "sqlDA.Fill error,fill null dt! storedProcName:" + storedProcName+",OconState:"+ocon.State.ToString());
                }
            }

            return dataSet;
        }

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行存储过程，将运行结果存入已有DataSet对象的指定表中，无返回值 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>  
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">存储过程的参数对象列表（数组）</param>
        /// <param name="dataSet">DataSet对象</param>
        /// <param name="tableName">数据表名称</param>
        public void RunProcedure(string storedProcName, IDataParameter[] parameters, DataSet dataSet, string tableName)
        {
                OracleDataAdapter sqlDA = new OracleDataAdapter();
            try
            {
                Open();

                OracleTransaction myOracleTransaction = ocon.BeginTransaction();

                sqlDA.SelectCommand = BuildCommand(storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                myOracleTransaction.Commit();
                Close();
            }
            catch (Exception e)
            {
                Open();
                sqlDA.Fill(dataSet, tableName);
                Close();
               // HGERROR.log(e.ToString() + "OracleTransaction error,rebuild execute!");
            }
            
        }
        #endregion

        #region 执行带参数的存储过程,返回DataTable
        /// <summary>
        /// 带参数的存储过程,返回DataTable
        /// </summary>
        /// <param name="ProcName">存储过程名称</param>
        /// <param name="parameters">参数集合</param>
        /// <returns>返回datatable对象</returns>
        public DataTable RunProcedure2DT(string ProcName, OracleParameter[] parameters)
        {
            DataTable dt;
            Oracledbhelp db = new Oracledbhelp();
            try
            {
                DataSet da = db.RunProcedure(ProcName, parameters, "t1");

                dt = da.Tables["t1"];

            }
            catch (Exception e)
            {
               // HGERROR.log(e + "存储过程:" + ProcName);
               
                dt = null;
            }
            finally
            {
                db.Close();
                db = null;
            }
            if (dt == null)
            {
                dt = new DataTable();
            }
            return dt;
        }

       

        #endregion

        /// <summary>
        /// 返回存储过程的输出字符串
        /// </summary>
        /// <param name="ProcName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string RunProcedure2OutString(string ProcName, OracleParameter[] parameters)
        {
            string outpar = "";
            Oracledbhelp db = new Oracledbhelp();
            try
            {
                db.RunProcedure(ProcName, parameters);
                outpar = (string)parameters[parameters.Length - 1].Value.ToString();

            }
            catch (Exception e)
            {
                
            }
            finally
            {
                db.Close();
                db = null;
            }
            return outpar;

        }

        #region 运行SQL语句
        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行与写数据库相关的SQL语句，返回影响行数 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>  
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响行数</returns>
        public int ExecNonQuery(string sqlString)
        {
            int RowAffected;
            //if(ocon.State.ToString() == "Closed") Open();
            Open();
            OracleCommand command = new OracleCommand(sqlString, ocon);
            RowAffected = command.ExecuteNonQuery();
            Close();

            return RowAffected;

        }

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行SQL语句，返回OracleDataReader对象 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>SqlDataReader对象</returns>
        public OracleDataReader ExecSqlString(string sqlString)
        {
            OracleDataReader returnReader;

            //if(ocon.State.ToString() == "Closed") Open();
            Open();
            OracleCommand command = new OracleCommand(sqlString, ocon);
            returnReader = command.ExecuteReader();
            //Close();

            return returnReader;
        }


        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行SQL语句，返回DataSet对象 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        /// <param name="string">SQL语句</param>
        /// <param name="tableName">数据表名称</param>
        /// <returns>DataSet对象</returns>
        public DataSet ExecSqlString(string sqlString, string tableName)
        {
            DataSet dataSet = new DataSet();
            //if (ocon.State.ToString() == "Closed") Open();
            Open();
            OracleTransaction myOracleTransaction = ocon.BeginTransaction();
           
            OracleDataAdapter sqlDA = new OracleDataAdapter();
            sqlDA.SelectCommand = new OracleCommand(sqlString, ocon);
            sqlDA.Fill(dataSet, tableName);
            myOracleTransaction.Commit();
            Close();

            return dataSet;
        }

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行SQL语句，将运行结果存入已有DataSet对象的指定表中，无返回值 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>  
        /// <param name="sqlString">SQL语句</param>
        /// <param name="dataSet">DataSet对象</param>
        /// <param name="tableName">数据表名称</param>
        public void ExecSqlString(string sqlString, DataSet dataSet, string tableName)
        {
            //if (ocon.State.ToString() == "Closed") Open();
            Open();
            OracleDataAdapter sqlDA = new OracleDataAdapter();
            sqlDA.SelectCommand = new OracleCommand(sqlString, ocon);
            sqlDA.Fill(dataSet, tableName);
            Close();
        }

        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：运行SQL语句，返回查询结果的第一行的第一列，忽略其它行或列 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>  
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响行数</returns>
        public object ExecScalar(string sqlString)
        {
            object returnScalar;
            //if (ocon.State.ToString() == "Closed") Open();
            Open();
            OracleCommand command = new OracleCommand(sqlString, ocon);
            returnScalar = command.ExecuteScalar();
            //Close();

            return returnScalar;
        }
        #endregion







        #region 传入输入参数
        /// <summary>
        /// 传入输入参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param></param>
        /// <param name="Size">参数大小</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的 parameter 对象</returns>
        public OracleParameter MakeInParam(string ParamName, OracleDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }
        #endregion




        #region 传入返回值参数
        /// <summary>
        /// 传入返回值参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新的 parameter 对象</returns>
        public OracleParameter MakeOutParam(string ParamName, OracleDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }
        #endregion

        #region 传入返回值参数
        /// <summary>
        /// 传入返回值参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新的 parameter 对象</returns>
        public OracleParameter MakeOutRecord(string ParamName, OracleDbType DbType)
        {
            return MakeParam(ParamName, OracleDbType.RefCursor, 0, ParameterDirection.Output, null);
        }


        #endregion





        #region 传入返回值参数
        /// <summary>
        /// 传入返回值参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <returns>新的 parameter 对象</returns>
        public OracleParameter MakeReturnParam(string ParamName, OracleDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.ReturnValue, null);
        }
        #endregion





        #region 生成存储过程参数
        /// <summary>
        /// 生成存储过程参数
        /// </summary>
        /// <param name="ParamName">存储过程名称</param>
        /// <param name="DbType">参数类型</param>
        /// <param name="Size">参数大小</param>
        /// <param name="Direction">参数方向</param>
        /// <param name="Value">参数值</param>
        /// <returns>新的 parameter 对象</returns>
        public OracleParameter MakeParam(string ParamName, OracleDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            OracleParameter param;

            if (Size > 0)
                param = new OracleParameter(ParamName, DbType, Size);
            else
                param = new OracleParameter(ParamName, DbType);

            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
                param.Value = Value;

            return param;
        }
        #endregion

        #region  打开数据库连接
        /// <summary>
        ///  打开数据库连接
        /// </summary>
        public void Open()
        {
            try
            {
                // 实例化 SqlConnection 对象
                if (ocon == null)
                {
                    ocon = new OracleConnection(connectionString);
                }
                // 打开数据库连接
                if (ocon.State == System.Data.ConnectionState.Closed)
                {
                    //HGERROR.log("ocon.state:" + ocon.State.ToString());
                    ocon.Open();

                }

                // 如果前面打开失败，再次打开数据库连接
                if (ocon.State != System.Data.ConnectionState.Open)
                {
                   // HGERROR.log("ocon.state:" + ocon.State.ToString());
                    ocon.Close();
                    ocon.Open();

                }
            }
            catch (Exception e)
            {
               // HGERROR.log(e.ToString() + "ocon.state:" + ocon.State.ToString());
                ocon = new OracleConnection(connectionString);
                ocon.Open();
               // HGERROR.log("ocon rebuild:" + ocon.State.ToString());
            }

        }
        #endregion





        #region 关闭数据库连接
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            if (ocon != null)
            {
                if (ocon.State == System.Data.ConnectionState.Open)
                {
                    ocon.Close();
                    ocon.Dispose();
                    ocon = null;
                }
            }


        }
        # endregion



        #region 析构函数
        /// <summary><table style="font-size:12px">
        /// <tr><td><b>功能描述</b>：析构函数，善后处理，释放数据连接 </td></tr>
        /// <tr><td><b>创 建 人</b>： </td></tr>
        /// <tr><td><b>创建时间</b>：</td></tr>
        /// </table></summary>
        ~Oracledbhelp()
        {
            if (ocon != null)
            {
                if (ocon.State.ToString() == "Open")
                    Close();
                ocon.Dispose();
            }
        }
        #endregion


    }
}

