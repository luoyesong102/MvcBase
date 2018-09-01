using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    /// <summary>
    /// 公司基础异常基类
    /// </summary>
    public class TpoBaseException : Exception
    {
        private int _code = 1;//异常代码
        public TpoBaseException(string message)
            : base(message)
        {
           
        }

        public TpoBaseException(string propertyName, string message)
            : base(message)
        {
            this.PropertyName = propertyName;
        }
        public TpoBaseException(int code, string message)
            : base(message)
        {
            this._code = code;
        }

        public TpoBaseException(int code, string propertyName, string message)
            : base(message)
        {
            this._code = code;
            this.PropertyName = propertyName;
        }


        public string PropertyName { get; set; }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public override string Source
        {
            get
            {
                return base.Source;
            }
            set
            {
                base.Source = value;
            }
        }

        public override string StackTrace
        {
            get
            {
                return base.StackTrace;
            }
        }

        public override System.Collections.IDictionary Data
        {
            get
            {
                return base.Data;
            }
        }

        public override string HelpLink
        {
            get
            {
                return base.HelpLink;
            }
            set
            {
                base.HelpLink = value;
            }
        }
        /// <summary>
        /// 异常错误代码
        /// </summary>
        public int Code { get { return _code; } set { _code = value; } }


    }

    /// <summary>
    /// 用户未登陆异常
    /// </summary>
    public  class UserNotLogInException:Exception
    {
        public override string Message
        {
            get
            {
                return "用户信息丢失，请重新登录";
            }
        }
    }


    /// <summary>
    /// 用户pageID错误
    /// </summary>
    public class PageNotFindException : Exception
    {
        public override string Message
        {
            get
            {
                return "用户来源Url和获取权限不匹配";
            }
        }
    }



}
