

using Esmart.Framework.Model;

namespace Esmart.Framework.Utilities
{
    public class ApiMessageManager
    {
        public static ResponseModel<T> SetResponse<T>(T obj)
        {
            ResponseModel<T> mr = new ResponseModel<T>();
            mr.Header.ReturnCode = (int)ServerErrcodeEnum.Normal;
            mr.Body = obj;
            return mr;
        }

        public static RequestModel<T> SetRequest<T>(T obj)
        {
            RequestModel<T> mr = new RequestModel<T>();
            mr.Body = obj;
            return mr;
        }

        public static SoaDataPageResponse<T> SetResponsePage<T>(T obj)
        {
            SoaDataPageResponse<T> mr = new SoaDataPageResponse<T>();
            mr.Header.ReturnCode= (int)ServerErrcodeEnum.Normal;
            return mr;
        }

        public static SoaDataPage<T> SetRequestPage<T>(T obj)
        {
            SoaDataPage<T> mr = new SoaDataPage<T>();
            return mr;
        }
    }
}