using System;


namespace Esmart.Framework.Aop
{
    public class Logger
    {
       

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get;
            set;
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public int LogType
        {
            get;
            set;
        }

        /// <summary>
        /// 请求的方法
        /// </summary>
        public string MethodName
        {
            get;
            set;
        }

        /// <summary>
        /// 请求方法的参数
        /// </summary>
        public string Args
        {
            get;
            set;
        }

        /// <summary>
        /// 对应的项目
        /// </summary>
        public string Project
        {
            get;
            set;
        }

        /// <summary>
        /// 来源类名
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// 时间
        /// </summary>
       
        public DateTime CreateDate
        {
            get;
            set;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        
        public DateTime EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// 用时
        /// </summary>
        public int UseTime
        {
            get;
            set;
        }

        /// <summary>
        /// 返回值
        /// </summary>
        public string ReturnValue
        {
            get;
            set;
        }
    }
}
