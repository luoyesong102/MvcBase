using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Esmart.Framework.Exceptions
{
    public class BusinessException:ApplicationException
    {
         public BusinessException()
            : base()
        {

        }
        public BusinessException(string message)
            : base(message)
        {

        }

        public BusinessException(string message, ServerErrcodeEnum _errCode)
            : base(message)
        {
            ErrorCode = _errCode;
        }
      

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ServerErrcodeEnum ErrorCode
        {
            set;
            get;
        }
    }
}
