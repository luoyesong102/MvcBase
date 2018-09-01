
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    /// <summary>
    /// 自定义特性 属性或者类可用  支持继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
    public class DBAttribute : Attribute 
    {
        public DBAttribute()
        {
            IsMapping = true; 
        }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 是否映射
        /// </summary>
        public bool IsMapping { get; set; }

        /// <summary>
        /// 是否为关键字
        /// </summary>
        public bool IsKey { get; set; }
    }
}
