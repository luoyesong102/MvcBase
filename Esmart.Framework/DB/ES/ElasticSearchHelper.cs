using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlainElastic.Net;
using PlainElastic.Net.Queries;
using PlainElastic.Net.Serialization;
namespace Esmart.Framework.DB
{
   public class ElasticSearchHelper
    {
        public static readonly ElasticSearchHelper Intance = new ElasticSearchHelper();
        //http://www.51xuediannao.com/c_asp_net/plainelastic.html  
        private ElasticConnection Client;

        private ElasticSearchHelper()
        {
            Client = new ElasticConnection("localhost", 9200);
        }

        /// <summary>
        /// 数据索引
        /// </summary>       
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="id">索引文档id，不能重复,如果重复则覆盖原先的</param>
        /// <param name="jsonDocument">要索引的文档,json格式</param>
        /// <returns>索引结果</returns>
        public IndexResult Index(string indexName, string indexType, string id, string jsonDocument)
        {
          
            var serializer = new JsonNetSerializer();

            string cmd = new IndexCommand(indexName, indexType, id);

            OperationResult result = Client.Put(cmd, jsonDocument);

            var indexResult = serializer.ToIndexResult(result.Result);

            return indexResult;
        }

        /// <summary>
        /// 数据索引
        /// </summary>       
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="id">索引文档id，不能重复,如果重复则覆盖原先的</param>
        /// <param name="jsonDocument">要索引的文档,object格式</param>
        /// <returns>索引结果</returns>
        public IndexResult Index(string indexName, string indexType, string id, object document)
        {
            var serializer = new JsonNetSerializer();

            var jsonDocument = serializer.Serialize(document);

            return Index(indexName, indexType, id, jsonDocument);
        }

        /// <summary>
        /// 全文检索
        /// </summary>
        /// <typeparam name="T">搜索类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="indexType">索引类型</param>
        /// <param name="query">查询条件（单个字段或者多字段或关系）</param>
        /// <param name="from">当前页（0为第一页）</param>
        /// <param name="size">页大小</param>
        /// <returns>搜索结果</returns>
        public SearchResult<T> Search<T>(string indexName, string indexType, QueryBuilder<T> query, int from, int size)
        {
            var queryString = query.From(from).Size(size).Build();

            var cmd = new SearchCommand(indexName, indexType);

            var result = Client.Post(cmd, queryString);
         
            var serializer = new JsonNetSerializer();

            return serializer.ToSearchResult<T>(result);
        }

    }
}
