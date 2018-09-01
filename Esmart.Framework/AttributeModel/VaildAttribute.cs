using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{

    /// <summary>
    /// 必须字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
        public RequiredAttribute(string errorMessage)
        {
            this.ErrorMessage = errorMessage;

        }


        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }


    
    /// <summary>
    /// 正则表达式验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RegexAttribute : Attribute
    {
        /// <summary>
        /// pattern类型，消息错误提示
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="errorMessage"></param>
        public RegexAttribute(string pattern, string errorMessage)
        {
            this.Pattern = pattern;
            this.ErrorMessage = errorMessage;
        }


        /// <summary>
        /// 验证类型
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
