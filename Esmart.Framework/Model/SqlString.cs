using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Esmart.Framework.Model
{
    /// <summary>
    /// sql查询对象
    /// </summary>
    public class SqlString
    { 
       
        /// <summary>
        /// 唯一ID
        /// </summary>
        [Esmart.Framework.Model.PrimarykeyAttrbute]
        public Int64 ID { get; set; }

        /// <summary>
        /// 运用程序ID
        /// </summary>
        public int AppID { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// sql语句
        /// </summary>
        public string SqlContent { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public Int64 CreateID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public Int64? ReviewID { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ReviewDate { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<SqlParam> SqlParam { get; set; }

    }

    /// <summary>
    /// sql查询参数
    /// </summary>
    public class SqlParam
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public Int64 ID { get; set; }

        /// <summary>
        /// 关联sql查询ID
        /// </summary>
        public Int64 SqlStringID { get; set; }

        /// <summary>
        /// sql查询参数比如:and ID=12
        /// </summary>
        public string ParamContent { get; set; }

        /// <summary>
        /// sql从外面传递过来的参数,如@Name  和对象名一样
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }

    }


    [Serializable]
    public class SqlContent
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// sql内容
        /// </summary>
        [XmlElement("SqlString")]
        public string Content { get; set; }

        /// <summary>
        /// sql参数
        /// </summary>
         [XmlElement("Param")]
        public string Parameter { get; set; }
    }


    [XmlRoot(ElementName = "Sqls")]
    public class SqlDocument
    {
        [XmlElement("Sql")]
        public SqlContent[] Entities { get; set; }
    }

    [Serializable]
    public class SqlEntity
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
       

        [XmlElement("SqlString")]
        public string Content { get; set; }

        [XmlElement("Param")]
        public SqlEntityParam[] Parameters { get; set; }
    }

    [Serializable]
    public class SqlEntityParam
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
      
    }
}
