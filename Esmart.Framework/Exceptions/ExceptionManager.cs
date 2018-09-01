using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmart.Framework.Logging;
using Esmart.Framework.Model;

namespace Esmart.Framework.Exceptions
{

    // MessageResponse 错误代码 0或者空正常 1服务内部异常 2服务安全校验失败，3业务异常
    //UserValidException 1 表示用户名密码错误，其他表示token无效
    public class ExceptionManager
    {
        public static MessageResponse<T> SetException<T>(Exception ex)  
        {
            
            MessageResponse<T> rValue = new MessageResponse<T>();
            if (ex is BusinessException || ex is ApplicationException||ex is TpoBaseException)
            {
                BusinessException bEx = ex as BusinessException;
                rValue.Code = bEx.ErrorCode;
                rValue.Message = ex.Message;
                rValue.Data = default(T);

                LogTxt.WriteLog("BusinessException"+bEx.ErrorCode, (int)LogType.Other);
            }
            else
            {
                rValue.Code = ServerErrcodeEnum.ServiceError;//系统异常
                rValue.Message = "服务未处理异常";
                rValue.Data = default(T);
                Exception logException = ex;
                if (ex.InnerException != null)
                    logException = ex.InnerException;
                LogTxt.WriteLog("BusinessException" + logException.Message, (int)LogType.Other);
               
            }
            return rValue;
        }
        public static MessageResponsePage<T> SetExceptionPage<T>(Exception ex)
        {

            MessageResponsePage<T> rValue = new MessageResponsePage<T>();
            if (ex is BusinessException || ex is ApplicationException)
            {
                BusinessException bEx = ex as BusinessException;
                rValue.Code = bEx.ErrorCode;
                rValue.Message = ex.Message;
              

                LogTxt.WriteLog("BusinessException" + bEx.ErrorCode, (int)LogType.Other);
            }
            else
            {
                rValue.Code = ServerErrcodeEnum.ServiceError;//系统异常
                rValue.Message = "服务未处理异常";
                Exception logException = ex;
                if (ex.InnerException != null)
                    logException = ex.InnerException;
                LogTxt.WriteLog("BusinessException" + logException.Message, (int)LogType.Other);

            }
            return rValue;
        }

        public static ResponseModel<T> SetResponseException<T>(Exception ex)
        {

            ResponseModel<T> rValue = new ResponseModel<T>();
            if (ex is TpoBaseException || ex is ApplicationException)
            {
                TpoBaseException bEx = ex as TpoBaseException;
                rValue.Header.ReturnCode = bEx.Code;
                rValue.Header.Message = ex.Message;
                rValue.Body = default(T);

                LogTxt.WriteLog("BusinessException" + bEx.Code, (int)LogType.Other);
            }
            else
            {
                rValue.Header.ReturnCode =(int) ServerErrcodeEnum.ServiceError;//系统异常
                rValue.Header.Message = "服务未处理异常";
                rValue.Body = default(T);
                Exception logException = ex;
                if (ex.InnerException != null)
                    logException = ex.InnerException;
                LogTxt.WriteLog("BusinessException" + logException.Message, (int)LogType.Other);

            }
            return rValue;
        }
        public static SoaDataPageResponse<T> SetResponseExceptionPage<T>(Exception ex)
        {

            SoaDataPageResponse<T> rValue = new SoaDataPageResponse<T>();
            if (ex is TpoBaseException || ex is ApplicationException)
            {
                TpoBaseException bEx = ex as TpoBaseException;
                rValue.Header.ReturnCode = bEx.Code;
                rValue.Header.Message = ex.Message;
                LogTxt.WriteLog("BusinessException" + bEx.Code, (int)LogType.Other);
            }
            else
            {
                rValue.Header.ReturnCode = (int)ServerErrcodeEnum.ServiceError;//系统异常
                rValue.Header.Message = "服务未处理异常";
                Exception logException = ex;
                if (ex.InnerException != null)
                    logException = ex.InnerException;
                LogTxt.WriteLog("BusinessException" + logException.Message, (int)LogType.Other);

            }
            return rValue;
        }
    }
}
