using System;

namespace Esmart.Framework.RabbitMq
{
  

    public class ConsumerReceiveMessage
    {
        public string MessageID { get; set; }

        public string MessageTitle { get; set; }

        public string MessageBody { get; set; }

        public string MessageRouter { get; set; }
        public string MessageQueue { get; set; }
        public string MessageExchange { get; set; }
    }
}
