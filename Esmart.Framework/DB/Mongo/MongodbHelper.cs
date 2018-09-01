using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Linq.Expressions;
using MongoDB.Bson;
using System.Reflection;
using System.IO;
using Esmart.Framework.Logging;
using Esmart.Framework.Exceptions;

namespace Esmart.Framework.DB
{
    public class MongodbHelper
    {
        /* MongoDB连接字符串,缓存数据库（MongoDB）服务地址
         * mongodb://192.168.1.31:22001|TransDBMongo
             mongodb://[username:password@]hostname[:port][/[database][?options]]
             */
        private static string[] connectionString = null;
        public static string ConnString = string.Empty;
        public static string DatabaseName = string.Empty;

        public void GetConn(string DBName)
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[DBName].ConnectionString.Split('|');
            ConnString = connectionString[0] + "/" + connectionString[1]; //"mongodb://192.168.1.6:27017";
            DatabaseName = connectionString[1]; //"sesamespell";

        }

    }

    public sealed class MongodbHelper<T> : MongodbHelper where T : class
    {
        private MongoServer mServer;//线程安全
        private MongoDatabase mDatabase;//线程安全
        private MongoCollection<T> mCollection;//线程安全
        private MongoClient mClient;//线程安全

        /// <summary>
        /// MongoDB带参数的构造函数
        /// </summary>
        /// <param name="databaseName">类似于RDMS的数据库名</param>
        /// <param name="collectionName">类似于RDMS的表名</param>
        public MongodbHelper(string collectionName, string DBName)
        {
            if (DBName != DatabaseName)
            {
                GetConn(DBName);
            }

            /* GetCollection 维系了之前返回过的一个实例表，
             * 如果以同样的参数再次调用 GetCollection 会得到同一个实例*/
            this.mClient = new MongoClient(ConnString);
            this.mServer = mClient.GetServer();
            this.mDatabase = mServer.GetDatabase(DatabaseName);
            this.mCollection = mDatabase.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Insert操作
        /// 应用无需维护：处于根级文档的实体类的id字段，MongoDB会自动为id赋值
        /// </summary>
        public T Insert(T t)
        {
            T info = null;
            try
            {
                WriteConcernResult result = this.mCollection.Insert<T>(t, WriteConcern.Acknowledged);//安全的写操作

                //if (result.Ok && !result.HasLastErrorMessage)
                if (!result.HasLastErrorMessage)
                {
                    info = t;
                }
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("Insert: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;
            }
            return info;
        }

        /// <summary>
        /// Insert操作
        /// 应用无需维护：处于根级文档的实体类的id字段，MongoDB会自动为id赋值
        /// </summary>
        public bool Insert(IList<T> t)
        {
            //IList<T> list = null;
            bool res = false;
            try
            {
                IEnumerable<WriteConcernResult> result = this.mCollection.InsertBatch<T>(t, WriteConcern.Acknowledged);//安全的写操作
                //list = t;

                if (result.Count() > 0)
                    res = true;

                //if (result.Ok && !result.HasLastErrorMessage)
                {
                    //info = t;
                    //result.Response.
                }
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("Insert(List): 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);

                res = false;
                //list = null;
            }
            return res;
        }

        public IList<T> InsertBatch(IList<T> t)
        {
            IList<T> list = null;
            //bool res = false;
            try
            {
                IEnumerable<WriteConcernResult> result = this.mCollection.InsertBatch<T>(t, WriteConcern.Acknowledged);//安全的写操作

                list = t;
                if (result.Any(p => p.HasLastErrorMessage))
                {
                    IEnumerable<WriteConcernResult> _result = result.Where(p => p.HasLastErrorMessage).Select(p => p);
                    LogTxt.WriteLog("InsertBatch错误结果: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + _result.ToJson(), (int)LogType.Mongodb);
                }
                //if (result.Count() > 0)
                //res = true;

                //if (result.Ok && !result.HasLastErrorMessage)
                //{
                    //info = t;
                    //result.Response.
                //}
                //Log.WriteLog("InsertBatch: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 结果：" + Newtonsoft.Json.JsonConvert.SerializeObject(result), (int)LogType.Track);
            }
            catch (MongoWriteConcernException e)
            {
                list = null;
                LogTxt.WriteLog("InsertBatch: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + Newtonsoft.Json.JsonConvert.SerializeObject(e), (int)LogType.Mongodb);
            }
            catch (Exception ex)
            {
                //res = false;
                list = null;
                LogTxt.WriteLog("InsertBatch: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);

            }
            return list;
        }

        public long InsertBulk(IList<T> t, string backFileName, string primaryKey = "Id")
        {
            long res = 0;
            int tryCount = 0;
        TaskStar:
            try
            {
                //Log.WriteLog(Newtonsoft.Json.JsonConvert.SerializeObject(t), (int)LogType.Track);

                BulkWriteOperation<T> bulk = this.mCollection.InitializeOrderedBulkOperation();
                foreach (var item in t)
                {
                    bulk.Insert(item);
                }
                BulkWriteResult bulkResult = bulk.Execute();
                res = bulkResult.InsertedCount;
                //Log.WriteLog("InsertBulk→结果：" + Newtonsoft.Json.JsonConvert.SerializeObject(bulkResult), (int)LogType.Track);
            }
            catch (MongoBulkWriteException e)
            {
                LogTxt.WriteLog("InsertBulk→错误信息：" + e.Message, (int)LogType.Mongodb);
                if (tryCount == 0)
                {
                    LogTxt.WriteLog(Newtonsoft.Json.JsonConvert.SerializeObject(t), backFileName, "MongoData");
                }

                if (tryCount < 3 && System.Text.RegularExpressions.Regex.IsMatch(e.Message, @"E11000 duplicate key error collection", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    tryCount++;
                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(e.Message, @"ObjectId\(\'(?<Id>[a-zA-Z0-9]+?)\'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        bool isAdd = false;
                        int num = t.Count;
                        IList<T> t2 = new List<T>();
                        for (var ii = 0; ii < num; ii++)
                        {
                            if (!isAdd)
                            {
                                PropertyInfo p = t[ii].GetType().GetProperty(primaryKey);
                                if (p != null && p.GetValue(t[ii], null).ToString() == m.Groups["Id"].Value)
                                {
                                    ObjectId newId = ObjectId.GenerateNewId();
                                    p.SetValue(t[ii], newId, null);
                                    t2.Add(t[ii]);
                                    isAdd = true;
                                }
                            }
                            else
                            {
                                t2.Add(t[ii]);
                            }
                        }
                        if (t2.Count > 0)
                        {
                            t = t2;
                            goto TaskStar;
                        }
                    }
                }
                res = -1;
            }
            catch (Exception ex)
            {
                res = -2;
                LogTxt.WriteLog("InsertBulk: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
            }

            return res;
        }

        public T FindOne(IMongoQuery query)
        {
            try
            {
                if (query != null && query != Query.Null)
                {
                    T model = this.mCollection.FindOne(query);
                    return model;
                }
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindOne→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
            }
            return null;
        }

        /// <summary>
        /// FindAll操作，返回符合条件的集合
        /// <param name="limit">限制游标返回结果集记录数,当limit=0时返回符合条件的全部记录</param>
        /// <returns>查询结果集</returns>
        /// </summary>
        public IList<T> FindAll(int limit, IMongoSortBy sort = null, string[] fields = null)
        {
            try
            {
                MongoCursor<T> mCursor = this.mCollection.FindAll();
                mCursor.SetLimit(limit);

                if (sort != null)
                    mCursor.SetSortOrder(sort);
                if (fields != null && fields.Length > 0)
                    mCursor.Fields = Fields.Include(fields);

                List<T> list = new List<T>();
                foreach (T item in mCursor)
                    list.Add(item);

                return list;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindAll: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;
            }
        }

        /// <summary>
        /// FindAsByWhere操作，返回符合条件的集合
        /// <param name="queryExpression">查询条件</param>
        /// <param name="limit">限制游标返回结果集记录数,当limit=0时返回符合条件的全部记录</param>
        /// <returns>查询结果集</returns>
        /// </summary>
        public IList<T> FindAsByWhere(Expression<Func<T, bool>> queryExpression, int limit, IMongoSortBy sort = null, string[] fields = null)
        {
            var query = Query.Null;
            if (queryExpression != null)
                query = Query<T>.Where(queryExpression);
            try
            {
                MongoCursor<T> mCursor = this.mCollection.Find(query);
                mCursor.SetLimit(limit);

                if (sort != null)
                    mCursor.SetSortOrder(sort);
                if (fields != null && fields.Length > 0)
                    mCursor.Fields = Fields.Include(fields);

                List<T> list = new List<T>();
                foreach (T item in mCursor)
                    list.Add(item);

                return list;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindAsByWhere(Expression)→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;

            }
        }

        /// <summary>
        /// FindAsByWhere操作，返回符合条件的集合
        /// <param name="query">查询条件</param>
        /// <param name="limit">限制游标返回结果集记录数,当limit=0时返回符合条件的全部记录</param>
        /// <returns>查询结果集</returns>
        /// </summary>
        public IList<T> FindAsByWhere(IMongoQuery query, int limit, IMongoSortBy sort = null, string[] fields = null)
        {
            MongoCursor<T> mCursor = null;
            List<T> list = null;
            try
            {

                if (limit == 1)
                {
                    T t = this.mCollection.FindOne(query);
                    if (t != null)
                    {
                        list = new List<T>();
                        list.Add(t);
                    }
                    return list;
                }
                else
                    mCursor = this.mCollection.Find(query);

                if (sort != null)
                    mCursor.SetSortOrder(sort);

                mCursor.SetLimit(limit);
                if (fields != null && fields.Length > 0)
                    mCursor.Fields = Fields.Include(fields);

                list = new List<T>();
                foreach (T item in mCursor)
                    list.Add(item);
                return list;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindAsByWhere→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;

            }

        }


        /// <summary>
        /// 返回符合条件的集合，带分页
        /// </summary>
        /// <param name="queryExpression">查询条件 Lambda表达式</param>
        /// <param name="recordCountcount">总记录数</param>
        /// <param name="sort">排序</param>
        /// <param name="page">页码</param>
        /// <param name="itemCount">每页记录数</param>
        /// <returns></returns>
        public IList<T> FindAsPage(Expression<Func<T, bool>> queryExpression, out Int64 recordCountcount, IMongoSortBy sort = null, Int32 page = 0, Int32 itemCount = 0, string[] fields = null)
        {
            sort = sort ?? new SortByDocument { };
            itemCount = (itemCount == 0) ? 1 : itemCount;

            var query = Query.Null;
            if (queryExpression != null)
                query = Query<T>.Where(queryExpression);
            recordCountcount = 0;
            MongoCursor<T> mCursor = null;
            try
            {
                if (page < 1)
                    mCursor = ((query == null) ? this.mCollection.FindAll() : this.mCollection.Find(query)).SetSortOrder(sort);
                else
                    mCursor = ((query == null) ? this.mCollection.FindAll() : this.mCollection.Find(query)).SetSortOrder(sort).SetSkip((page - 1) * itemCount).SetLimit(itemCount);

                recordCountcount = ((query == null) ? this.mCollection.FindAll() : this.mCollection.Find(query)).Count();

                if (fields != null && fields.Length > 0)
                    mCursor.Fields = Fields.Include(fields);


                List<T> list = new List<T>();
                foreach (T item in mCursor)
                    list.Add(item);
                return list;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindAsPage→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;
            }
        }


        /// <summary>
        /// 返回符合条件的集合，带分页
        /// </summary>
        /// <param name="queryExpression">查询条件 Lambda表达式</param>
        /// <param name="recordCountcount">总记录数</param>
        /// <param name="sort">排序</param>
        /// <param name="page">页码</param>
        /// <param name="itemCount">每页记录数</param>
        /// <returns></returns>
        public IList<T> FindAsPage(IMongoQuery query, out Int64 recordCountcount, IMongoSortBy sort = null, Int32 page = 0, Int32 itemCount = 0, string[] fields = null)
        {
            sort = sort ?? new SortByDocument { };
            itemCount = (itemCount == 0) ? 1 : itemCount;

            recordCountcount = 0;
            MongoCursor<T> mCursor = null;
            try
            {
                if (page < 1)
                    mCursor = ((query == null) ? this.mCollection.FindAll() : this.mCollection.Find(query)).SetSortOrder(sort);
                else
                    mCursor = ((query == null) ? this.mCollection.FindAll() : this.mCollection.Find(query)).SetSortOrder(sort).SetSkip((page - 1) * itemCount).SetLimit(itemCount);

                recordCountcount = ((query == null) ? this.mCollection.FindAll() : this.mCollection.Find(query)).Count();

                if (fields != null && fields.Length > 0)
                    mCursor.Fields = Fields.Include(fields);


                List<T> list = new List<T>();
                foreach (T item in mCursor)
                    list.Add(item);
                return list;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindAsPage(IMongoQuery)→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;
            }
        }


        /// <summary>
        /// 获取满足条件的记录数
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public long FindAsCount(IMongoQuery query)
        {
            long count = 0;
            try
            {
                count = query == null ? (long)(this.mCollection.FindAll().Count()) : (long)(this.mCollection.Find(query).Count());
                return count;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("FindAsCount→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return count;
            }
        }

        /// <summary>
        /// Update操作
        /// <param name="queryExpression">查询条件</param>
        /// <param name="vList">更新set值</param>
        /// <param name="queryExpression">更新条件</param>
        /// <returns>-1输入参数不匹配，Update操作影响的文档数</returns>
        /// </summary>
        public long UpdateDocs(Expression<Func<T, bool>> queryExpression, List<object> vList, params Expression<Func<T, object>>[] updateExpression)
        {
            var query = Query.Null;
            if (queryExpression != null)
                query = Query<T>.Where(queryExpression);

            List<Expression<Func<T, object>>> expressionList = updateExpression.ToList<Expression<Func<T, object>>>();
            if (expressionList.Count != vList.Count)
                return -1;

            List<IMongoUpdate> iMongoUpdateList = new List<IMongoUpdate>();
            for (int i = 0; i < expressionList.Count; i++)
            {
                iMongoUpdateList.Add(Update<T>.Set(expressionList[i], vList[i]));
            }
            var update = Update<T>.Combine(iMongoUpdateList);
            try
            {
                WriteConcernResult wcr = this.mCollection.Update(query, update, UpdateFlags.Multi, WriteConcern.Acknowledged);
                return wcr.DocumentsAffected;//Gets the number of documents affected.
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("UpdateDocs1→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return 0;
            }
        }

        public long UpdateDocs(IMongoQuery query, IMongoUpdate update)
        {
            WriteConcernResult wcr = this.mCollection.Update(query, update, UpdateFlags.Multi, WriteConcern.Acknowledged);
            return wcr.DocumentsAffected;
        }

        /// <summary>
        /// Update操作
        /// </summary>
        /// <param name="queryExpression">查询条件</param>
        /// <param name="update"></param>
        /// <returns>Update操作影响的文档数</returns>
        public long UpdateDocs(Expression<Func<T, bool>> queryExpression, IMongoUpdate update)
        {
            var query = Query.Null;
            if (queryExpression != null)
                query = Query<T>.Where(queryExpression);
            try
            {
                WriteConcernResult wcr = this.mCollection.Update(query, update, UpdateFlags.Multi, WriteConcern.Acknowledged);
                return wcr.DocumentsAffected;//Gets the number of documents affected.
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("UpdateDocs2→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return 0;
            }
        }

        /// <summary>
        /// Remove操作
        /// <param name="queryExpression">匹配条件</param>
        /// <returns>Remove操作影响的文档数</returns>
        /// </summary>
        public long RemoveDocs(Expression<Func<T, bool>> queryExpression)
        {
            var query = Query.Null;
            if (queryExpression != null)
                query = Query<T>.Where(queryExpression);
            try
            {
                WriteConcernResult wcr = this.mCollection.Remove(query, WriteConcern.Acknowledged);
                return wcr.DocumentsAffected;//Gets the number of documents affected.
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("RemoveDocs1→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return 0;
            }
        }

        public long RemoveDocs(IMongoQuery query)
        {
            try
            {
                WriteConcernResult wcr = this.mCollection.Remove(query, WriteConcern.Acknowledged);
                return wcr.DocumentsAffected;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("RemoveDocs2→Query：" + query.ToString() + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return 0;
            }
        }
        /// <summary>
        /// Distinct操作
        /// </summary>
        /// <param name="Key">列:可嵌套</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public List<string> getDistinct(string key, IMongoQuery query)
        {
            return this.mCollection.Distinct<string>(key, query).ToList();
        }

        public bool InsertOrUpdate(IMongoQuery query, IMongoUpdate update)
        {
            //FindAndModifyResult famr = this.mCollection.FindAndModify(query, null, update, false, true);
            FindAndModifyArgs args = new FindAndModifyArgs()
            {
                Query = query,
                Update = update,
                SortBy = null,
                Upsert = true,
                VersionReturned = FindAndModifyDocumentVersion.Original
            };
            FindAndModifyResult famr = this.mCollection.FindAndModify(args);
            return famr.Ok;
        }

        public IEnumerable<BsonDocument> GroupBy(GroupArgs args)
        {
            try
            {
                IEnumerable<BsonDocument> list = this.mCollection.Group(args);
                return list;
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("GroupBy: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;
            }
        }

        public IEnumerable<BsonDocument> Aggregate(AggregateArgs args)
        {
            try
            {
                return this.mCollection.Aggregate(args);
            }
            catch (Exception ex)
            {
                LogTxt.WriteLog("Aggregate: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.Source + "\r\n" + ex.TargetSite + "\r\n" + ex.InnerException + "\r\n" + ex.StackTrace, (int)LogType.Mongodb);
                return null;
            }
        }
    }

    //public static class Logger
    //{
    //    /// <summary>
    //    /// 写日志
    //    /// </summary>
    //    /// <param name="LogStr">日志内容</param>
    //    public static void WriteLog(string LogStr, int i = 0)
    //    {
    //        try
    //        {
    //            LogStr = DateTime.Now.ToString("HH:mm:ss:fff") + "　" + LogStr;
    //            string strPath = AppDomain.CurrentDomain.BaseDirectory + "log"; //HttpRuntime.AppDomainAppPath+"log";
    //            strPath = strPath + "\\Mongodb";
    //            if (!System.IO.Directory.Exists(strPath))
    //                System.IO.Directory.CreateDirectory(strPath);
    //            strPath = strPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

    //            using (StreamWriter sw = new StreamWriter(strPath, true, System.Text.Encoding.Default))
    //            {
    //                sw.WriteLine(LogStr);
    //                sw.Flush();
    //                sw.Close();
    //            }
    //        }
    //        catch
    //        {
    //        }
    //    }
    //}

}
