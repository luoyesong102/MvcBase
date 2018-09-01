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
using System.Linq;
using System.Text;
using Esmart.Framework.Model;

namespace Esmart.Framework.DB
{
    /// <summary>
    /// 基于链式表达拼接sql语句
    /// </summary>
    public interface ISmartDbExec
    {
        /// <summary>
        /// 1、可以根据特性来生成sql语句，特新中有Esmart.Framework.Model.GreaterThan,Esmart.Framework.Model.LessThan,Esmart.Framework.Model.FuzzyQuery,Esmart.Framework.Model.QueryIgnoreAttribute,Esmart.Framework.Model.OrQuery,Esmart.Framework.Model.LessThan
        /// 2、查询条件中model的属性名字或者属性别名和数据库表字段一致
        /// </summary>
        /// <param name="model">查询条件中model的属性名字或者属性别名和数据库表字段一致</param>
        /// <returns>返回当前对象</returns>
        ISmartDbExec Query(object model);

        /// <summary>
        ///1、生成  and(  语句
        ///2、必须和RightBracke语句使用，不然sql语句会出异常
        ///3、比如 AndLeftBrackets().AndQuery(new {userID=12,Name="test"}).RightBrackets()  拼接成的sql语句是 and (userid=12 and name='test' )
        /// </summary>
        /// <returns></returns>
        ISmartDbExec AndLeftBrackets();


        /// <summary>
        ///1、生成   or (  语句
        ///2、必须和RightBracke语句使用，不然sql语句会出异常
        ///3、比如 OrLeftBrackets().AndQuery(new {userID=12,Name="test"}).RightBrackets()  拼接成的sql语句是 or (userid=12 and name='test' )
        /// </summary>
        /// <returns></returns>
        ISmartDbExec OrLeftBrackets();


        /// <summary>
        ///  1、必须和AndLeftBrackets()或者OrLeftBrackets()方法一起使用
        /// </summary>
        /// <returns></returns>
        ISmartDbExec RightBrackets();


        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  AndQuery(new {userID=12,Name="test"}) 拼接成 and userid=12 and name='test'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec AndQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  OrQuery(new {userID=12,Name="test"}) 拼接成 or userid=12 or name='test'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec OrQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  OrLessThanQuery(new {userID=12,Name="test"}) 拼接成 or userid&lt;=12 or name&lt;='test'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec OrLessThanQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  AndLessThanQuery(new {userID=12,Name="test"}) 拼接成 and userid&lt;=12 and name&lt;='test'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        ISmartDbExec AndLessThanQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  OrGreaterThanQuery(new {userID=12,Name="test"}) 拼接成 or userid>=12 or name>='test'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec OrGreaterThanQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  AndGreaterThanQuery(new {userID=12,Name="test"}) 拼接成 and userid>=12 and name>='test'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec AndGreaterThanQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  AndFuzzyQuery(new {userID=12,Name="test"}) 拼接成 and userid like '%12%" and name like '%test%'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec AndFuzzyQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  OrFuzzyQuery(new {userID=12,Name="test"}) 拼接成 or userid like '%12%"  or Name like '%test%'
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ISmartDbExec OrFuzzyQuery(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句,model中的属性是List&lt;值类型> ,List&lt;string> ,值类型[]，string[] 当中的一种
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  AndQueryIn(new {userID=new List&lt;int>(){1,2,3},Name=new List&lt;int>(){"1","2","3"}}) 拼接成 and userid in (1,2,3) and  Name in('1','2','3')
        /// </summary>
        ISmartDbExec AndQueryIn(object obj);

        /// <summary>
        /// 1、根据model的属性拼接sql语句,model中的属性是List&lt;值类型> ,List&lt;string> ,值类型[]，string[] 当中的一种
        /// 2、model的属性名字或者别名要和字段名字一致
        /// 3、比如  OrQueryIn(new {userID=new List&lt;int>(){1,2,3},Name=new List&lt;int>(){"1","2","3"}}) 拼接成 in userid in (1,2,3) in  Name in('1','2','3')
        /// </summary>
        ISmartDbExec  OrQueryIn(object obj);

       /// <summary>
       ///1、 2张表 存在in 查询 ，如 select  *  from A  where  Id in(select ID1  from  b where 1=1)
        ///2、参数 mianId 表示的是ID，tableId表示 ID1，如果 ID=ID1 ，则不需要填写tableId
       /// </summary>
       /// <typeparam name="Tobj">传递删除的类型</typeparam>
       /// <param name="mianId">主表的ID</param>
       /// <param name="obj">参数的对象</param>
       /// <param name="tableId">in(里面的id)</param>
       /// <returns></returns>
        ISmartDbExec AndInTableQuery<Tobj>(string mianId, object obj, string tableId = null);


        /// <summary>
        ///1、 2张表 存在in 查询 ，如 select  *  from A  where  Id in(select ID1  from  b where 1=1)
        ///2、参数 mianId 表示的是ID，tableId表示 ID1，如果 ID=ID1 ，则不需要填写tableId
        /// </summary>
        /// <typeparam name="Tobj">传递删除的类型</typeparam>
        /// <param name="mianId">主表的ID</param>
        /// <param name="obj">参数的对象</param>
        /// <param name="tableId">in(里面的id)</param>
        /// <returns></returns>
        ISmartDbExec OrInTableQuery<Tobj>(string mianId, object obj, string tableId = null);

        /// <summary>
        /// 1、根据链式查询条件返回集合
        /// 2、指定的对象类型,对象类型和数据库表完全依赖，依赖规则类型字段名称或者别名和数据库字段名字一致，类型名字或者别名和数据库表明一致
        /// 3、不在保存链式查询的条件，如果要使用Print()方法，必须先调用print，然后再执行ToList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> ToList<T>();

        /// <summary>
        /// 1、根据链式查询条件删除数据，如果存在isdelete  ，则为逻辑删除，设置isdelete=1
        /// 2、T的名字或者别名必须和数据库表名字一致
        /// 3、不在保存链式查询的条件，如果要使用Print()方法，必须先调用print，然后再执行Delete
        /// </summary>
        /// <typeparam name="T">类型名字或者别名对应数据库表的名字</typeparam>
        /// <returns></returns>
        int Delete<T>();

       /// <summary>
        /// 1、根据链式查询条件修改数据，不为null的数据才会修改
        /// 2、T的名字或者别名必须和数据库表名字一致
        /// 3、修改数据的时候，如果对obj特性设置了基本验证，则会先通过验证，在执行方法
        /// 4、不在保存链式查询的条件，如果要使用Print()方法，必须先调用print，然后再执行Update
       /// </summary>
       /// <typeparam name="T">T的名字或者别名必须和数据库表名字一致</typeparam>
        /// <param name="obj">要修改的数据，obj的名字或者别名必须和数据库表名字一致</param>
       /// <returns></returns>
        ResponseHeader Update<T>(T obj);

        /// <summary>
        /// 1、新添加数据，不为null的数据才会添加
        /// 2、T的名字或者别名必须和数据库表名字一致
        /// 3、添加数据的时候，如果对model特性设置了基本验证，则会先通过验证，在执行方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseHeader Add<T>(T model);

        /// <summary>
        /// 根据链式表达式，打印出具体的sql语句
        /// </summary>
        /// <returns></returns>
        string Print();

        /// <summary>
        /// 1、根据链式表达式查询数据是否存在
        /// 2、T的名字或者别名必须和数据库表名字一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool Exist<T>();

        /// <summary>
        /// 1、根据链式表达式查询数据的总数
        /// 2、T的名字或者别名必须和数据库表名字一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int Count<T>();

    }
}
