/*
 ===============================================================================
 * 作者        ：chalei.wu
 * 编写时间    :2014-11-23
 * 修改历史记录：
 * 存在的bug   ：
 * 待优化方案  ：
 ===============================================================================
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Esmart.Framework.Model;

namespace Esmart.Framework.DB
{
    /// <summary>
    ///  操作设计的删除字段如果类型属性存在IsDelete，数据表示逻辑删除（0表示没删除，1表示删除）否则物理删除
    ///  对数据库操作的类型 必须制定唯一key，而且必须只能为单一key
    ///  对类型枚举的时候，数据库对应的是枚举value，不是枚举的name
    /// </summary>
    public interface IDbExec : ISmartDbExec
    {
        /// <summary>
        /// ADO.NET原始操作
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para">sql参数</param>
        /// <returns></returns>
        int ExecuteSql(string sql, DbParameter[] para = null);


        /// <summary>
        /// 执行存储过程，且返回结果
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="procedureName">存储过程的名字</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        T ExecuteProcedure<T>(string procedureName, object where);


        /// <summary>
        /// 1、新增一条数据
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、如果存在自增字段，返回当前自增Id，否则返回0,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>如果存在自增字段，返回当前自增Id，否则返回0,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int InsertObject<Tobj>(Tobj obj);


        /// <summary>
        /// 1、新增一条数据
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、如果存在自增字段，返回当前自增Id，否则返回0,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>如果存在自增字段，返回当前自增Id，否则返回0,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        void InsertObjectAsy<Tobj>(Tobj obj);

        /// <summary>
        /// 1、批量新增对象
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、如果存在自增字段，返回当前自增Id的集合，否则返回空的集合,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>如果存在自增字段，返回当前自增Id的集合，否则返回空的集合,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        List<int> InsertListObject<Tobj>(IEnumerable<Tobj> obj);

        /// <summary>
        /// 1、修改一个对象(字段为null不修改)
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int UpdateObject<Tobj>(Tobj obj);


        /// <summary>
        /// 1、强制修改全部字段
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int UpdateObjectAllField<Tobj>(Tobj obj);

        /// <summary>
        /// 1、批量修改(字段为null不修改)
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int UpdateListObject<Tobj>(IEnumerable<Tobj> obj);



        /// <summary>
        /// 1、强制全部修改对象集合
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="obj">指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致</param>
        /// <returns>返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int UpdateListObjectAllField<Tobj>(IEnumerable<Tobj> obj);

        /// <summary>
        /// 1、删除一个对象，id为主键，如果对象类型属性存在IsDelete，则修改IsDelete为1，否则删除数据
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、对象类型设定的唯一key，每个对象类型都必须设置唯一的key
        /// 4、返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="id">对象类型设定的唯一key，每个对象类型都必须设置唯一的key</param>
        /// <returns>返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int DeleteObj<Tobj>(object id);


        /// <summary>
        /// 1、批量删除ids为主键集合,如果对象类型属性存在IsDelete，则修改IsDelete为1，否则删除数据
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、对象类型设定的唯一key，每个对象类型都必须设置唯一的key
        /// 4、返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="ids">对象类型设定的唯一key，每个对象类型都必须设置唯一的key</param>
        /// <returns>返回影响的行数,如果失败或者发生异常，都会抛出异常，需要调用方捕获异常</returns>
        int DeleteListObj<Tobj>(IEnumerable<object> ids);



        /// <summary>
        /// 1、单表对象查询。如果有多条记录，取第一条
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、返回单一的具体对象
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="model">model可以是唯一key，也可以是查询条件，查询条件中的model属性名字或者别名必须和数据库字段一致</param>
        /// <returns>返回单一的具体对象</returns>
        Tobj Find<Tobj>(object model);

        /// <summary>
        /// 根据用户自定义方法筛选（对单一表有效）
        /// </summary>
        /// <typeparam name="Tobj">返回类型</typeparam>
        /// <param name="functions">用户定义的方法</param>
        /// <returns></returns>
        Tobj Find<Tobj>(System.Linq.Expressions.Expression<Func<Tobj, bool>> functions) where Tobj : class;





        /// <summary>
        /// 1、单表集合查询,查询出所有没有被删除的数据
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、返回出所有没有被删除的数据
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <returns>返回出所有没有被删除的数据</returns>
        List<Tobj> FindAll<Tobj>();



        /// <summary>
        /// 根据用户自定义方法筛选（对单一表有效）
        /// </summary>
        /// <typeparam name="Tobj">返回类型</typeparam>
        /// <param name="functions">用户定义的方法</param>
        /// <returns></returns>
        List<Tobj> FindAll<Tobj>(System.Linq.Expressions.Expression<Func<Tobj, bool>> functions) where Tobj : class;

        /// <summary>
        /// 1、单表集合查询
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、条件查询，查询条件中的model属性名字或者别名必须和数据库字段一致
        /// 4、返回满足条件没有被删除的集合
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="model">条件查询，查询条件中的model属性名字或者别名必须和数据库字段一致</param>
        /// <returns>返回满足条件没有被删除的集合</returns>
        List<Tobj> FindAll<Tobj>(object model);



        /// <summary>
        /// 1、单表集合查询
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、具体的sqlwhere语句，如：where  userid=12
        /// 4、返回满足条件没有被删除的集合
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="where">具体的sqlwhere语句，如：where  userid=12</param>
        /// <param name="para">sql参数</param>
        /// <returns>返回满足条件没有被删除的集合</returns>
        List<Tobj> FindListByWhere<Tobj>(string where, DbParameter[] para);


        /// <summary>
        /// 1、单表分页查询
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致
        /// 3、返回分页没有被删除的集合
        /// </summary>
        /// <typeparam name="Tobj">指定的对象类型,对象类型和数据库表完全依赖，类型名字或者别名和数据库表明一致</typeparam>
        /// <param name="page">分页条件</param>
        /// <param name="count">返回满足条件的总数</param>
        /// <returns>返回分页没有被删除的集合</returns>
        List<Tobj> FindList<Tobj>(DBPage page, out int count);



        /// <summary>
        /// 1、自定义sql语句查询
        /// 2、sql语句返回字段必须和类型属性或者别名一致
        /// 3、返回满足条件的集合
        /// </summary>
        /// <typeparam name="Tobj">sql语句返回字段必须和类型属性或者别名一致</typeparam>
        /// <param name="strSQL">ssql语句返回字段必须和类型属性或者别名一致</param>
        /// <param name="para">sql参数</param>
        /// <returns>返回满足条件的集合</returns>
        List<Tobj> FindList<Tobj>(string strSQL, DbParameter[] para = null);


        /// <summary>
        /// 1、按照配置文件中的name获取具体的sql语句
        /// 2、配置文件文件夹为XmlConfig,文件类型为*.xml,格式为&lt;?xml version="1.0" encoding="utf-8" ?><Sqls><Sql Name="clueInfoGuidIn" Type="SQL" WithTrascation="false"><SqlString> sql语句 </SqlString> <Param Name="test">sql参数条件</Param></Sql></Sqls>
        /// 3、查询类型中的属性或者别名对应xml中参数的名字
        /// 4、返回类型的属性名字或者别名和sql语句中返回的字段名字一致
        /// 5、返回满足条件的集合
        /// 6、xml特殊字符&lt;(小于)&lt;>(大于)&gt; "(双引号)  &quot;'(单引号)  &apos;
        /// <typeparam name="TReturn">指定的返回对象类型,返回类型的属性名字或者别名和sql语句中返回的字段名字一致</typeparam>
        /// <param name="name">配置文件xml文档中的name</param>
        /// <param name="tmodel">对象中的属性或者别名对应xml中参数的名字</param>
        /// <returns>返回满足条件的集合</returns>
        /// </summary>
        List<TReturn> FindListByName<TReturn>(string name, object tmodel, Func<string, string> injectionSql = null, bool userSqlContent = false);

        /// <summary>
        /// 1、按照配置文件中的name获取具体的sql语句
        /// 2、配置文件文件夹为XmlConfig,文件类型为*.xml,格式为&lt;?xml version="1.0" encoding="utf-8" ?><Sqls><Sql Name="clueInfoGuidIn" Type="SQL" WithTrascation="false"><SqlString> sql语句 </SqlString> <Param Name="test">sql参数条件</Param></Sql></Sqls>
        /// 3、查询类型中的属性或者别名对应xml中参数的名字
        /// 4、返回类型的属性名字或者别名和sql语句中返回的字段名字一致
        /// 5、返回分页的集合
        /// 6、xml特殊字符&lt;(小于)&lt;>(大于)&gt; "(双引号)  &quot;'(单引号)  &apos;
        /// </summary>
        /// <returns>返回分页的集合</returns>
        List<TReturn> FindListByName<TReturn>(string name, DBPage tmodel, out  int count, Func<string, string> injectionSql = null, bool userSqlContent = false);


        /// <summary>
        /// 1、根据自定义的sql语句分页查询数据
        /// 2、返回类型的属性名字或者别名和sql语句中返回的字段名字一致
        /// 3、返回分页的集合
        /// </summary>
        /// <param name="sql">用户自定义的sql语句</param>
        /// <param name="tmodel">tmodel.Where的属性名字或者别名配置文件中参数的名字一致</param>
        /// <param name="count">满足条件的总数</param>
        /// <param name="dbPar">用户定义的参数</param>
        /// <returns>返回分页的集合</returns>
        List<TReturn> FindListBySql<TReturn>(string sql, DBPage tmodel, out  int count, DbParameter[] dbPar = null);




        /// <summary>
        ///1、根据用户定义的参数来拼接sql
        ///2、如果用户中的属性为null，用户定义的查询条件and name=@name 就会变成and 1=1  
        /// </summary>
        /// <param name="sql">用户定义的sql语句</param>
        /// <param name="model">用户一定的参数</param>
        /// <param name="dbPar">用户一定的参数</param>
        /// <returns></returns>
        string CreateSqlByParSql(string sql, object model, ref List<DbParameter> dbPar);



        /// <summary>
        /// 原始操作执行
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string strSQL, DbParameter[] para = null);

        /// <summary>
        /// 返回单一数字
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        object ExecuteScalar(string strSQL, DbParameter[] para = null);


        /// <summary>
        /// 连接字符串Key,对应配置文件的Key
        /// </summary>
        string ConnectKey { get; set; }

        /// <summary>
        /// 创建一个事务连接
        /// </summary>
        /// <returns></returns>
        ITransactionDbExec CreateTransaction();

    }


    /// <summary>
    /// 接口针对DB的增，修，删
    /// 尽量使用2次以上的sql操作
    /// </summary>
    public interface ITransactionDbExec : IDisposable
    {
        /// <summary>
        /// 事务提交
        /// </summary>
        /// <returns>输出受影响的行数</returns>
        int Commit();



        /// <summary>
        /// 新添加一个对象
        /// </summary>
        /// <typeparam name="T">对应数据库对象的实体类</typeparam>
        /// <param name="model">实体类的具体事例</param>
        ITransactionDbExec TransactionAdd<T>(T model) where T : class;


        /// <summary>
        /// 修改对象
        /// </summary>
        /// <typeparam name="T">对应数据库对象的实体类</typeparam>
        /// <param name="model">实体类的具体事例</param>
        /// <param name="exec">链式查询条件</param>
        ITransactionDbExec TransactionUpdate<T>(T model, ISmartDbExec exec = null) where T : class;



        /// <summary>
        /// 删除一个对象
        /// </summary>
        ITransactionDbExec TransactionDelete<T>(object id, ISmartDbExec exec = null) where T : class;


    }



    /// <summary>
    /// 主要为sql性能考虑
    /// </summary>
    public interface IDbSelect
    {
         SoaDataPageResponse<T> FindList<T>(string sql, SoaDataPage page, DbParameter[] dbPar = null) where T:new();

         List<T> FindList<T>(string sql, DbParameter[] dbPar = null) where T:new();

         bool IgnoreCase
         {
             get;
             set;
         }
    }
}
