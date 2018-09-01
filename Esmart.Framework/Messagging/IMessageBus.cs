using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Messagging
{
    interface IMessageBus
    { 
        void Pubish(IMessage message);
        void RunIt();
        void StopIt();
    }
}
