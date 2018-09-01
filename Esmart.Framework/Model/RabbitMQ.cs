using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public class RabbitMQ
    {
        /// <summary>
        /// 头结点
        /// </summary>
        public RabbitMQHeader Header { get; set; }

        /// <summary>
        /// 参数内容
        /// </summary>
        public object Body { get; set; }
    }

    public  class  RabbitMQHeader
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string ActionID { get; set; }


        /// <summary>
        /// 内容
        /// </summary>
        public object Message { get; set; }
    }
}
