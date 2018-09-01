using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public class JsonReturn
    {
        public JsonHeader Header { get; set; }

        public object Content { get; set; }

        public JsonReturn()
        {
            Header = new JsonHeader();
            Header.Success = true;
        }
    }

    public  class  JsonHeader
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回说明性文字
        /// </summary>
        public string Message { get; set; }

    }
}
