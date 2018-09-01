using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Esmart.Framework.Caching;
using Esmart.Framework.Logging;
using Esmart.Framework.Model;

namespace Esmart.Framework.Soa
{
    [System.Web.Services.WebServiceBindingAttribute(Name = "WebRequestSoap", Namespace = "http://tpooo.com/")]
    internal class HttpSOAClient : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        public List<RequestInfo> GetList()
        {
            ICache cache = CacheManager.CreateRedisCache();
            string key = typeof(RequestInfo).FullName + GlobalConfig.AppID;
            List<RequestInfo> list = cache.Get<List<RequestInfo>>(key);
            if (list == null || list.Count == 0)
            {
                list = GetDefaultList();
                cache.Set(key, list);
            }
            return list;
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tpooo.com/Request", RequestNamespace = "http://tpooo.com/", ResponseNamespace = "http://tpooo.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Request(string requestType, string requestbody, string url, int timeout)
        {
            this.Url = url;
            this.Timeout = timeout;
            object[] results = this.Invoke("Request", new object[] {
                        requestType,requestbody});
            return ((string)(results[0]));
        }

        public void RequsetAsy(string requestType, string request, string url, int timeout)
        {
            this.Url = url;
            this.Timeout = timeout;
            this.InvokeAsync("Request", new object[] {
                       requestType ,request}, null, null);
        }


        public List<RequestInfo> GetDefaultList()
        {
            List<RequestInfo> list = null;
            try
            {
                RequestModel<int> model = new RequestModel<int>();
                model.Body = Convert.ToInt32(GlobalConfig.AppID ?? "0");
                string soaUrl = string.Format("http://{0}/WebDefault.asmx", GlobalConfig.SoaDomain.Trim());
                string response = Request("Default", model.Body.ToString(), soaUrl, 100000);
                ResponseModel<List<RequestInfo>> responseModel = JsonConvert.DeserializeObject<ResponseModel<List<RequestInfo>>>(response);
                list = responseModel.Body;
            }
            catch (Exception ex)
            {
                LogManager.CreateTpoLog().Error("RequestInfo信息获取失败", ex);
                list = new List<RequestInfo>();
            }
            return list;
        }

    }

    public class SOAClient
    {

        /// <summary>
        /// 调用soa服务
        /// </summary>
        /// <typeparam name="TRequest">请求对象</typeparam>
        /// <typeparam name="TResponse">返回对象</typeparam>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        public static ResponseModel<TResponse> Request<TRequest, TResponse>(RequestModel<TRequest> request)
        {

            string response = null;

            try
            {
                HttpSOAClient client = new HttpSOAClient();

                if (request.Header.AppID == 0)
                {
                    request.Header.AppID = ConstantDefine.AppID;
                }

                ResponseModel<TResponse> ReturnResponse = null;

                RequestInfo model = null;

                if (ConstantDefine.NotUsePlatForm)
                {
                    model = new RequestInfo()
                    {
                        RequestType = request.Header.RequestType.Trim(),
                        TimeOut = 60000,
                        RequestUrl = string.Format("http://{0}/WebRequest.asmx", GlobalConfig.SoaDomain.Trim())
                    };
                }
                else
                {
                    model = GetDefaultList(request.Header.RequestType);
                }
                if (model == null)
                {
                    throw new Exception("请开发人员去公共平台配置基础信息");
                }
                if (model != null)
                {
                    TResponse resonse = GetCache<TResponse>(model);
                    if (resonse != null && !string.IsNullOrEmpty(model.GetCacheKey))
                    {
                        return new ResponseModel<TResponse>() { Body = resonse };
                    }

                    string requestStr = string.Empty;

                    if (request.Body != null)
                    {
                        if (request.Body.GetType().IsClass && request.Body.GetType() != typeof(string))
                        {
                            requestStr = JsonConvert.SerializeObject(request.Body);
                        }
                        else
                        {
                            requestStr = request.Body.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(requestStr) && requestStr.Length <= UInt16.MaxValue)
                    {
                        LogManager.CreateTpoLog().Info(request.Header.GUID ?? request.Header.RequestType, requestStr);
                    }
                    response = client.Request(model.RequestType, requestStr, model.RequestUrl, model.TimeOut);
                    ReturnResponse = JsonConvert.DeserializeObject<ResponseModel<TResponse>>(response);
                    SetCache<TResponse>(model, ReturnResponse.Body);
                    ClearCache(model);
                    return ReturnResponse;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ResponseModel<TResponse> model = new ResponseModel<TResponse>();

                    if (!string.IsNullOrEmpty(response))
                    {
                        var returnobj = JsonConvert.DeserializeObject<ResponseModel<object>>(response);
                        model.Header = returnobj.Header;
                    }
                    else
                    {
                        model.Header.ReturnCode = -1;
                        model.Header.Message = ex.Message;
                    }
                  
                    return model;
                }
                catch (Exception ex1)
                {
                    ResponseModel<TResponse> model = new ResponseModel<TResponse>();
                    model.Header.ReturnCode = -1;
                    model.Header.Message = ex1.Message;
                    return model;
                }

            }
            throw new TpoBaseException("程序出现未知异常，请联系 开发人员");
        }


        /// <summary>
        /// 原生调用，用作测试
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="requestbody"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tpooo.com/Request", RequestNamespace = "http://tpooo.com/", ResponseNamespace = "http://tpooo.com/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public static string Request(string requestType, string requestbody, string url)
        {
            return new HttpSOAClient().Request(requestType, requestbody, url, 100000);
        }


        private static TResponse GetCache<TResponse>(RequestInfo model)
        {
            if (!string.IsNullOrEmpty(model.GetCacheKey) && !string.IsNullOrEmpty(ConstantDefine.RedisCacheAddress))
            {
                return CacheManager.CreateRedisCache().Get<TResponse>(model.GetCacheKey);
            }
            return default(TResponse);
        }


        private static void SetCache<TResponse>(RequestInfo model, TResponse response)
        {
            if (!string.IsNullOrEmpty(model.GetCacheKey) && !string.IsNullOrEmpty(ConstantDefine.RedisCacheAddress))
            {
                CacheManager.CreateRedisCache().Set(model.GetCacheKey, response);
            }
        }

        private static void ClearCache(RequestInfo model)
        {
            if (!string.IsNullOrEmpty(model.RemoveCacheKey) && !string.IsNullOrEmpty(ConstantDefine.RedisCacheAddress))
            {
                CacheManager.CreateRedisCache().Remove(model.RemoveCacheKey);
            }
        }

        /// <summary>
        /// 得到访问对象
        /// </summary>
        public static RequestInfo GetDefaultList(string requestType)
        {
            HttpSOAClient client = new HttpSOAClient();

            var data = client.GetList();

            var model = data.Find(a => a.RequestType.Trim() == requestType.Trim());

            if (model == null)
            {
                Esmart.Framework.Logging.LogManager.CreateTpoLog().Info(requestType, JsonConvert.SerializeObject(data));//日记记录
            }

            return model;
        }


        public static void RequestAsy<TRequest>(RequestModel<TRequest> request)
        {
            try
            {
                HttpSOAClient client = new HttpSOAClient();
                RequestInfo model = client.GetList().Find(a => a.RequestType == request.Header.RequestType);
                if (model != null)
                {
                    string requestStr = string.Empty;
                    if (request.Body != null)
                    {
                        if (request.Body.GetType().IsClass && request.Body.GetType() != typeof(string))
                        {
                            requestStr = JsonConvert.SerializeObject(request.Body);
                        }
                        else
                        {
                            requestStr = request.Body.ToString();
                        }
                    }
                    if (requestStr.Length <= UInt16.MaxValue)
                    {
                        LogManager.CreateTpoLog().Info(request.Header.RequestType, requestStr);
                    }
                    client.RequsetAsy(model.RequestType, requestStr, model.RequestUrl, model.TimeOut);
                }
            }
            catch (Exception ex)
            {
                LogManager.CreateTpoLog().Error(request.Header.RequestType, ex);
            }

        }
    }


    public class RequestHttpSOA
    {

        private static List<RequestInfo> GetList()
        {
            ICache cache = CacheManager.CreateNetCache();
            string key = typeof(RequestInfo).FullName;
            List<RequestInfo> list = cache.Get<List<RequestInfo>>(key);
            if (list == null || list.Count == 0)
            {
                list = GetDefaultList();
                cache.Set(key, list);
            }
            return list;
        }

        private static string Requset(string requestType, string requestbody, string url, int timeout)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = timeout;
            string param = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                        <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                          <soap:Body>
                                            <Request xmlns=""http://tpooo.com/"">
                                               <requestType>{0}</requestType>
                                               <requestbody>{1}</requestbody>
                                            </Request>
                                          </soap:Body>
                                        </soap:Envelope>";
            param = string.Format(param, requestType,requestbody);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml; charset=utf-8";
            byte[] bs = Encoding.UTF8.GetBytes(param);
            webRequest.Headers.Add("SOAPAction", "http://tpooo.com/Request");
            webRequest.ContentLength = bs.Length;
            using (Stream reqStream = webRequest.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (HttpWebResponse myResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                XmlDocument xml = new XmlDocument();
                xml.Load(sr);
                XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
                manager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                XmlNode node = xml.SelectSingleNode("soap:Envelope/soap:Body", manager);
                if (node != null)
                {
                    return node.InnerText;
                }
            }
            return string.Empty;
        }

        private static List<RequestInfo> GetDefaultList()
        {
            List<RequestInfo> list = null;
            try
            {
                string response = Requset("", GlobalConfig.AppID, GlobalConfig.SoaDomain, 100000);
                ResponseModel<List<RequestInfo>> responseModel = JsonConvert.DeserializeObject<ResponseModel<List<RequestInfo>>>(response);
                list = responseModel.Body;
            }
            catch (Exception ex)
            {
                LogManager.CreateTpoLog().Error("RequestInfo信息获取失败", ex);
                list = new List<RequestInfo>();
            }
            return list;
        }

        /// <summary>
        /// 调用soa服务
        /// </summary>
        /// <typeparam name="TRequest">请求对象</typeparam>
        /// <typeparam name="TResponse">返回对象</typeparam>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        public static ResponseModel<TResponse> Request<TRequest, TResponse>(RequestModel<TRequest> request)
        {
            string response = null;
            try
            {
                RequestInfo model = GetList().Find(a => a.RequestType.Trim() == request.Header.RequestType.Trim());
                ResponseModel<TResponse> ReturnResponse = null;
                if (model != null)
                {
                    TResponse resonse = GetCache<TResponse>(model);
                    if (resonse != null && !string.IsNullOrEmpty(model.GetCacheKey))
                    {
                        return new ResponseModel<TResponse>() { Body = resonse };
                    }

                    string requestStr = string.Empty;

                    if (request.Body != null)
                    {
                        if (request.Body.GetType().IsClass && request.Body.GetType() != typeof(string))
                        {
                            requestStr = JsonConvert.SerializeObject(request.Body);
                        }
                        else
                        {
                            requestStr = request.Body.ToString();
                        }
                    }

                    response = Requset(model.RequestType, requestStr, model.RequestUrl, model.TimeOut);


                    ReturnResponse = JsonConvert.DeserializeObject<ResponseModel<TResponse>>(response);

                    SetCache<TResponse>(model, ReturnResponse.Body);
                    ClearCache(model);
                    return ReturnResponse;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ResponseModel<TResponse> model = new ResponseModel<TResponse>();

                    if (!string.IsNullOrEmpty(response))
                    {
                        var returnobj = JsonConvert.DeserializeObject<ResponseModel<object>>(response);
                        model.Header = returnobj.Header;
                    }
                    else
                    {
                        model.Header.ReturnCode = 1;
                        model.Header.Message = ex.Message;
                    }
                    LogManager.CreateTpoLog().Error(request.Header.RequestType, ex);
                    return model;
                }
                catch (Exception ex1)
                {
                    ResponseModel<TResponse> model = new ResponseModel<TResponse>();
                    model.Header.ReturnCode = -1;
                    model.Header.Message = ex1.Message;
                    return model;
                }
            }
            return null;
        }


        private static TResponse GetCache<TResponse>(RequestInfo model)
        {
            if (!string.IsNullOrEmpty(model.GetCacheKey))
            {
                return CacheManager.CreateCache().Get<TResponse>(model.GetCacheKey);
            }
            return default(TResponse);
        }


        private static void SetCache<TResponse>(RequestInfo model, TResponse response)
        {
            if (!string.IsNullOrEmpty(model.GetCacheKey))
            {
                CacheManager.CreateCache().Set(model.GetCacheKey, response);
            }
        }

        private static void ClearCache(RequestInfo model)
        {
            if (!string.IsNullOrEmpty(model.RemoveCacheKey))
            {
                CacheManager.CreateCache().Remove(model.RemoveCacheKey);
            }
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="request"></param>
        public static void RequestAsy<TRequest>(RequestModel<TRequest> request)
        {
            RequestInfo model = GetList().Find(a => a.RequestType == request.Header.RequestType);
            if (model != null)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(a =>
                {
                    RequestInfo requestModel = a as RequestInfo;
                    Requset(model.RequestType, JsonConvert.SerializeObject(request.Body), requestModel.RequestUrl, requestModel.TimeOut);
                }, model);

            }
        }

    }

}
