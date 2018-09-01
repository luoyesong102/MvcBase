using System.Data.Common;
using System.Data;
using System.Collections.Generic;

namespace Esmart.Framework.DB.Dapper
{
    /// <summary>
    /// 提供对数据库的基本操作，连接字符串需要在数据库配置。
    /// </summary>
    public interface IDBHelper
    {
        /// <summary>
        /// 生成分页SQL语句
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="selectSql"></param>
        /// <param name="sqlCount"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        string GetPagingSql(int pageIndex, int pageSize, string selectSql, string sqlCount, string orderBy);

        #region 事务
        /// <summary>
        /// 开始一个事务
        /// </summary>
        /// <returns></returns>
        IDbTransaction BeginTractionand(IsolationLevel Iso = IsolationLevel.Unspecified);


        /// <summary>
        /// 开始一个事务
        /// </summary>
        /// <param name="connKey">数据库连接字符key</param>
        IDbTransaction BeginTractionand(string connKey, IsolationLevel Iso = IsolationLevel.Unspecified);

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="dbTransaction">要回滚的事务</param>
        void RollbackTractionand(IDbTransaction dbTransaction);

        /// <summary>
        /// 结束并确认事务
        /// </summary>
        /// <param name="dbTransaction">要结束的事务</param>
        void CommitTractionand(IDbTransaction dbTransaction);

        #endregion

        #region DataSet

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        DataSet ExecuteDataSet(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);


        /// <summary>
        ///  执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        DataSet ExecuteDataSet(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);


        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        DataSet ExecuteDataSet(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数dataset
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集</returns>
        DataSet ExecuteDataSet(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        int ExecuteNonQuery(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        int ExecuteNonQuery(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        int ExecuteNonQuery(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,只返回影响行数
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>影响的行数</returns>
        int ExecuteNonQuery(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        #endregion

        #region IDataReader

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回DataReader
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>DataReader</returns>
        IDataReader ExecuteReader(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        #endregion

        #region IEnumerable<T>

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> ExecuteIEnumerable<T>(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new();

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> ExecuteIEnumerable<T>(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new();

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> ExecuteIEnumerable<T>(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new();

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回IEnumerable<T>
        /// </summary>
        /// <typeparam name="T">返回类似</typeparam>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>IEnumerable</returns>
        IEnumerable<T> ExecuteIEnumerable<T>(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null) where T : class, new();

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        object ExecuteScalar(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        T ExecuteScalar<T>(string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        object ExecuteScalar(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="connKey">连接字符串Key</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        T ExecuteScalar<T>(string connKey, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        object ExecuteScalar(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);
        
        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="conn">要执行ＳＱＬ语句的连接</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        T ExecuteScalar<T>(IDbConnection conn, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数object．第一行，第一列的值
        /// </summary>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        object ExecuteScalar(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);

        /// <summary>
        /// 执行ＳＱＬ语句或者存储过程 ,返回参数类似T．第一行，第一列的值
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="trans">语句所在的事务</param>
        /// <param name="commandText">ＳＱＬ语句或者存储过程名</param>
        /// <param name="commandParameters">ＳＱＬ语句或者存储过程参数</param>
        /// <param name="commandType">ＳＱＬ语句类型</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>执行结果集第一行，第一列的值</returns>　
        T ExecuteScalar<T>(IDbTransaction trans, string commandText, List<IDataParameter> commandParameters = null, CommandType commandType = CommandType.Text, int? commandTimeout = null);
        
        #endregion

    }
}