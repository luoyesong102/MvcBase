using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public class MessageRequest<T>
    {
        public T Data { set; get; }

        public string InterFaceId { set; get; }
      
    }
}
