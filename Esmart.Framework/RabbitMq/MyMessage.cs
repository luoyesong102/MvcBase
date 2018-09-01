using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyNetQ;
namespace Esmart.Framework.RabbitMq
{
    [Queue("TestMessagesQueue", ExchangeName = "MyTestExchange")]
    public class MyMessage<T> 
    {
        public string MessageID { get; set; }
        
        public string MessageTitle { get; set; }

        public T MessageBody { get; set; }

        public string MessageRouter { get; set; }
        public string MessageQueue { get; set; }
        public string MessageExchange { get; set; }
        public string MessageExchangeType { get; set; }
    }


    public class Request<T>
    {
        public string MessageID { get; set; }

        public string MessageTitle { get; set; }

        public  T MessageBody { get; set; }

        public string MessageRouter { get; set; }
        public string MessageQueue { get; set; }
        public string MessageExchange { get; set; }
    }


    public class MyOtherMessage
    {
        public string Text { get; set; }
    }

}
