using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
namespace Esmart.Framework.DB
{
    [XmlRoot(ElementName="Sqls")]
    public class SqlDocument
    {
        [XmlElement("Sql")]
        public SqlEntity[] Entities { get; set; }
    }
    
    [Serializable]
    public class SqlEntity
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public QLType Type { get; set; }
        [XmlAttribute(AttributeName = "WithTrascation")]
        public bool WithTrasaction { get; set; }
        [XmlElement("SqlString")]
        public string Content { get; set; }
        [XmlElement("Params")]
        public SqlEntityParam[] Parameters { get; set; }
    }
    
    [Serializable]
    public class SqlEntityParam
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "DataType")]
        public string DataType { get; set; }
    }
}
