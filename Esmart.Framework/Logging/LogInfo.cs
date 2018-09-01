using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{

    [TableName(Name = "Log_{yyyyMMdd}_Info", IsUserDateCreateTable = true)]
    public class LogInfo
    {
        /// <summary>
        /// 唯一key
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 运用程序ID
        /// </summary>
        public int AppID { get; set; }

    }

    [TableName(Name = "Log_{yyyyMMdd}_Waring", IsUserDateCreateTable = true)]
    public class LogWaring
    {
        /// <summary>
        /// 唯一key
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 运用程序ID
        /// </summary>
        public int AppID { get; set; }


        public int UserId { get; set; }

    }

    [TableName(Name = "Log_Exception")]
    public class LogException
    {
        /// <summary>
        /// 唯一key
        /// </summary>
        public string Title { get; set; }



        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 运用程序ID
        /// </summary>
        public int AppID { get; set; }

        /// <summary>
        /// message信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 来源信息
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 跟踪信息
        /// </summary>
        public string StackTrace { get; set; }


        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }


       

    }



    [TableName(Name = "Log_Trace")]
    public class LogTrace
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// msg信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 总共时间
        /// </summary>
        public Int64 TotalTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 来源（0客户端，1服务端）
        /// </summary>
        public int FromType { get; set; }


        public int UserId { get; set; }
    }
    [TableName(Name = "Log_RequestInfo")]
    public class LogRequestInfo
    {
      

        public string Type { get; set; }

        public string Message { get; set; }

        public DateTime? CreateDate { get; set; }

        public string Message2 { get; set; }

        public string Message3 { get; set; }

        public string State { get; set; }

        public string LogType { get; set; }


        public string SortColumnName { get; set; }
        public string SortDirection { get; set; }

    }
    public class RequestInfo
    {
        /// <summary>
        /// 访问类型
        /// </summary>
        [FuzzyQuery]
        [PrimarykeyAttrbute]
        public string RequestType { get; set; }

        /// <summary>
        /// 服务是否使用缓存key
        /// </summary>
        public string GetCacheKey { get; set; }

        /// <summary>
        /// 服务是否清空缓存key
        /// </summary>
        public string RemoveCacheKey { get; set; }

        /// <summary>
        /// 程序集的全名称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 执行的名字
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 程序dll名字
        /// </summary>
        public string DDLName { get; set; }

        /// <summary>
        /// 程序ID
        /// </summary>
        public int AppID { get; set; }

        /// <summary>
        /// 访问地址
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestJson { get; set; }
    }
}
