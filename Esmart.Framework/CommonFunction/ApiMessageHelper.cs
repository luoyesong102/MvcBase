

using Esmart.Framework.Model;

namespace Esmart.Framework.Utilities
{
    public class ApiMessageHelper
    {
        public static MessageResponse<T> SetResponse<T>(T obj)
        {
            MessageResponse<T> mr = new MessageResponse<T>();
            mr.Code = ServerErrcodeEnum.Normal;
            mr.Data = obj;
            return mr;
        }

        public static MessageRequest<T> SetRequest<T>(T obj)
        {
            MessageRequest<T> mr = new MessageRequest<T>();
            mr.Data = obj;
            return mr;
        }

        public static MessageResponsePage<T> SetResponsePage<T>(T obj)
        {
            MessageResponsePage<T> mr = new MessageResponsePage<T>();
            mr.Code = ServerErrcodeEnum.Normal;
            mr.Message = "";
          
            return mr;
        }

        public static MessageRequestPage<T> SetRequestPage<T>(T obj)
        {
            MessageRequestPage<T> mr = new MessageRequestPage<T>();
          
            return mr;
        }
    }
}