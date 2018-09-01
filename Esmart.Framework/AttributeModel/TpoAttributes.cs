using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColNameAttribute : Attribute
    {
        /// <summary>
        /// 对应字段的名字
        /// </summary>
        public string Name { get; set; }
    }



    public class PrimarykeyAttrbute : Attribute
    {

    }


    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// 数据库表名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否按照时间格式来创建表，如果是,名字按照XXXX_{时间格式}_XXXX
        /// </summary>
        public bool IsUserDateCreateTable { get; set; }


    }

    /// <summary>
    /// 是否放在缓存中
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        ///设置的分钟数
        /// </summary>
        public int ExpiresAt { get; set; }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class RefColAttribute : Attribute
    {

    }


    public class DeleteAttrbute : Attribute
    {
        public string Name { get; set; }

        public int DeleteValue { get; set; }
    }

    /// <summary>
    /// 模糊查询
    /// </summary>
    public class FuzzyQuery : Attribute
    {

    }

    /// <summary>
    /// 大于查询
    /// </summary>
    public class GreaterThan : Attribute
    {

    }

    /// <summary>
    /// 小于查询
    /// </summary>
    public class LessThan : Attribute
    {

    }

    /// <summary>
    /// 或者的关系
    /// </summary>
    public class OrQuery : Attribute
    {

    }

    /// <summary>
    /// 避免登陆
    /// </summary>
    public class LoginIgnoreAttribute : Attribute
    {

    }


    /// <summary>
    /// 取消用户权限验证
    /// </summary>
    public class AuthorityIgnoreAttribute : Attribute
    {

    }

    /// <summary>
    /// 服务类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServerActionAttribute : Attribute
    {

        /// <summary>
        /// 服务类型(实现接口的服务)
        /// </summary>
        public Type ServerType { get; set; }

    }


    /// <summary>
    /// 查询取消条件
    /// </summary>
    public class QueryIgnoreAttribute : Attribute
    {

    }

    /// <summary>
    /// 表示非空
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VerificationAttribute : Attribute
    {
        bool _isNoEmpty = true;
        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool IsAllowEmpty
        {
            get { return _isNoEmpty; }
            set { _isNoEmpty = value; }
        }

        /// <summary>
        /// 正则表达式（参考Esmart.Framework.VerificationConstantDefine）
        /// </summary>
        public string RegularString { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public int MinSize { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public int MaxSize { get; set; }
    }

}
