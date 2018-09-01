using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmart.Framework.Messagging;
using System.IO;
using System.Xml.Serialization;
using Esmart.Framework.FileSystem;
using System.Reflection;
using Esmart.Framework.MyCache;
using Esmart.Framework.DB;
using Esmart.Framework.Exceptions;
namespace Esmart.Framework.FileSystem
{
    /// <summary>
    /// Sql的xml文件发生了改变时产生的Message
    /// </summary>
    public class SqlFileMessage : IMessage
    {
        protected static XmlSerializer _sqlDocSerializer = new XmlSerializer(typeof(SqlDocument));
       
        protected FileInfo _file;
        protected ICache<string, SqlDocument> _sqlCache;

        public SqlFileMessage() { }

        public SqlFileMessage(string filePath)
        {
            _file = new FileInfo(filePath);
          
            _sqlCache = MyCacheManager.GetCache<string, SqlDocument>(Constants.SQL_CACHE_KEY, k => SqlXmlHelper.Instance.GetDocument(k));
        }

        public FileInfo FileInfo
        {
            get
            {
                return _file;
            }
            set
            {
                _file = value;
            }
        }

        /// <summary>
        /// 处理这此消息，将改变的Xml重新序列化并保存到缓存中
        /// </summary>
        public virtual void ProcessMe()
        {
            CheckFile();
            using (var stream = File.OpenRead(_file.FullName))
            {
                SqlDocument doc;
                try
                {
                    doc = (SqlDocument)_sqlDocSerializer.Deserialize(stream);
                }
                catch (Exception)
                {
                    doc = null;
                }
                if (doc != null)
                {
                    _sqlCache.Set(_file.Name.TrimEnd(".xml".ToCharArray()), doc);
                }
            }
        }

        protected void CheckFile()
        {
            if (_file == null)
            {
                var ex = new Exception("未获取到文件信息！");
             
                throw ex;
            }

            if (!_file.Exists)
            {
                var ex = new FileNotFoundException("文件未找到", _file.FullName);
               
                throw ex;
            }
        }
    }

    /// <summary>
    /// 删除XML文件消息
    /// </summary>
    public class SqlFileDeleteMessage : SqlFileMessage
    {
        public SqlFileDeleteMessage(string filePath) : base(filePath) { }

        public sealed override void ProcessMe()
        {
            CheckFile();
            _sqlCache.Remove(_file.Name.TrimEnd(".xml".ToCharArray()));
        }
    }

    /// <summary>
    /// 重命名XML文件消息
    /// </summary>
    public class SqlFileRenameMessage : SqlFileMessage
    {
        private string _newPath, _oldPath;

        public SqlFileRenameMessage(string oldPath, string newPath)
        {
            _newPath = newPath;
            _oldPath = oldPath;
        }

        public sealed override void ProcessMe()
        {
            CheckFile();
            var newMessage = new SqlFileMessage(_newPath);
            var deleteMessage = new SqlFileDeleteMessage(_oldPath);
            MessageBus.Instance.Pubish(newMessage);
            MessageBus.Instance.Pubish(deleteMessage);
        }
    }
}
