using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esmart.Framework.RabbitMq
{
    public static class ExchangeType
    {
        public const string Direct = "direct";
        public const string Topic = "topic";
        public const string Fanout = "fanout";
        public const string Header = "headers";
    }
}
