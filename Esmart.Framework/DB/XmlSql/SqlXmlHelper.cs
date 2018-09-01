using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Esmart.Framework.MyCache;
using Esmart.Framework.Exceptions;
namespace Esmart.Framework.DB
{
    /// <summary>
    /// 用来解析保存SQL/HQL语句的Xml文件，并将其保存到缓存中
    /// </summary>
    public class SqlXmlHelper
    {
        private static XmlSerializer _sqlDocSerializer = new XmlSerializer(typeof(SqlDocument));
        private static SqlXmlHelper helper;
        private static readonly object _lock = new object();
        public static SqlXmlHelper Instance {
            get {
                //改为单例模式
                if (helper == null)
                {
                    lock (_lock)
                    {
                        if (helper == null)
                            helper = new SqlXmlHelper();
                    }
                }
                return helper;
            }
        }

        private ICache<string, SqlDocument> _sqlCache;

        public SqlXmlHelper()
        {
            _sqlCache = MyCacheManager.GetCache<string, SqlDocument>(Constants.SQL_CACHE_KEY, k => GetDocument(k));
        }

        /// <summary>
        /// 通过xml文件名，获取指定xml文件序列化后的SqlDocument对象
        /// </summary>
        /// <param name="xmlFileName">xml文件名</param>
        /// <returns>序列化后的对象</returns>
        public SqlDocument GetDocument(string xmlFileName)
        {
            string xmlName = !xmlFileName.EndsWith("xml", true, null) ? xmlFileName + ".xml" : xmlFileName;
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                          "XmlConfig",
                                          xmlName);
            if (!File.Exists(xmlPath))
            {
                throw new BusinessException("未找到SQL配置文件[" + xmlFileName+ "]或["+ xmlFileName +".xml]");
            }
            else
            {
                using (var stream = File.OpenRead(xmlPath))
                {
                    try
                    {
                        var doc = (SqlDocument)_sqlDocSerializer.Deserialize(stream);
                        if (doc == null)
                        {
                            throw new BusinessException("SQL配置文件解析错误！[解析结果为空]");
                        }
                        return doc;
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessException("SQL配置文件解析错误！");
                        
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 通过指定的xml文件名和Sql节点的Name属性值，获取序列化后的SqlEntity对象
        /// </summary>
        /// <param name="xmlFileName">xml文件名</param>
        /// <param name="sqlName">Sql节点的Name属性值</param>
        /// <returns>序列化后的对象</returns>
        public SqlEntity FindSQL(string xmlFileName, string sqlName)
        {
            var doc = _sqlCache.Get(xmlFileName);
            return doc.Entities.FirstOrDefault(e => e.Name.CompareTo(sqlName)==0);              
        }
    }
}
