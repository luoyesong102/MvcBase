  
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public class MessageResponse<T> 
    {
        /// <summary>
        /// 错误代码 0或者空正常 1服务内部异常 2服务安全校验失败，3业务异常
        /// </summary>
        public ServerErrcodeEnum Code { set; get; }

        public string Message { set; get; }

        public T Data { set; get; }
    }

  
} 
