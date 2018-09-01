using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Esmart.Framework.Infrastructor.Http
{
    public class WebRequestHelper
    {
        public static string HttpGetConnectToServer(string serverUrl, string postData)
        {
            //创建请求  
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl + "?" + postData);
            request.Method = "GET";
            //设置上传服务的数据格式  
            request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认  
            request.Credentials = CredentialCache.DefaultCredentials;
            //请求超时时间  
            request.Timeout = 10000;
            //读取返回消息  
            string res;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                return "{\"code\":1,\"message\":\"" + ex.Message + "\"}";// "{\"error\":\"connectToServer\",\"error_description\":\"" + ex.Message + "\"}";
            }
            return res;
        }

        public static string PostStr(string url, string jsonData)
        {

           
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            if (jsonData == "")
                req.Method = "GET";
            else
                req.Method = "POST";
            req.ContentType = "text/json";//"application/x-www-form-urlencoded";
            req.ContentLength = requestBytes.Length;
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
            string backstr = sr.ReadToEnd();


            sr.Close();
            res.Close();
            return backstr;
        }
        public static string PostStr(string url, string jsonData,int timeout)
        {
            System.GC.Collect();
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
            Stream requestStream = null ;
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            StreamReader sr = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = timeout;
            req.ReadWriteTimeout = timeout;
                req.KeepAlive = false;
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            if (jsonData == "")
                req.Method = "GET";
            else
                req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";//"";
            req.ContentLength = requestBytes.Length;
            requestStream = req.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();
            res = (HttpWebResponse)req.GetResponse();
            using (res = (HttpWebResponse)(HttpWebResponse)req.GetResponse())
            {
                if (res.StatusCode == HttpStatusCode.RequestTimeout)
                {

                    return "-1";
                }
                else
                {
                     sr= new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                    string backstr = sr.ReadToEnd();
                      sr.Close();
                      res.Close();
                      return backstr;
                }
            }
          
           }
                
            catch(Exception ex)
            {
                return "-1";
            }
            finally
            {
                if (sr!=null)
                { sr.Close(); }
                if (res != null)
                { res.Close(); }
                if (requestStream != null)
                { requestStream.Close(); }
                if (req != null)
                { req.Abort(); }
            }
           
        }

        public static string PostStrByJson(string url, string jsonData, int timeout)
        {
            System.GC.Collect();
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
            Stream requestStream = null;
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            StreamReader sr = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = timeout;
                req.ReadWriteTimeout = timeout;
                req.KeepAlive = false;
                byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
                if (jsonData == "")
                    req.Method = "GET";
                else
                    req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";//"";
                req.ContentLength = requestBytes.Length;
                requestStream = req.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
                res = (HttpWebResponse)req.GetResponse();
                using (res = (HttpWebResponse)(HttpWebResponse)req.GetResponse())
                {
                    if (res.StatusCode == HttpStatusCode.RequestTimeout)
                    {

                        return "-1";
                    }
                    else
                    {
                        sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                        string backstr = sr.ReadToEnd();
                        sr.Close();
                        res.Close();
                        return backstr;
                    }
                }

            }

            catch (Exception ex)
            {
                return "-1";
            }
            finally
            {
                if (sr != null)
                { sr.Close(); }
                if (res != null)
                { res.Close(); }
                if (requestStream != null)
                { requestStream.Close(); }
                if (req != null)
                { req.Abort(); }
            }

        }
        public static string GetStr(string url)
        {
            System.Net.HttpWebRequest req;
            System.Net.HttpWebResponse res;
            req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            res = (System.Net.HttpWebResponse)req.GetResponse();
            Encoding en = UnicodeEncoding.UTF8;
            System.IO.StreamReader strm = new System.IO.StreamReader(res.GetResponseStream(), en);
                //UnicodeEncoding.GetEncoding("GB2312"));
            return strm.ReadToEnd();
        }


        /// <summary>          
        /// 加载远程XML文档          
        /// </summary>         
        /// <param name="URL"></param>          
        /// <returns></returns>          
        public static XmlDocument ProcessXml(string URL)
        {

            Uri uri = new Uri(URL);
            WebRequest myReq = WebRequest.Create(uri);
            WebResponse result = myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(strHTML);
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return doc;
        }
        /// <summary>
        /// 加载远程html文档
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string ProcessItemHtml(string URL)
        {
            Uri uri = new Uri(URL);
            WebRequest myReq = WebRequest.Create(uri);
            WebResponse result = myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }




        public static string GetWebClient(string url)
        {
            string strHTML = "";
            WebClient myWebClient = new WebClient();
            Stream myStream = myWebClient.OpenRead(url);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
            strHTML = sr.ReadToEnd();
            myStream.Close();
            return strHTML;
        }

        public static string GetWebRequest(string url)
        {
            Uri uri = new Uri(url);
            WebRequest myReq = WebRequest.Create(uri);
            WebResponse result = myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }

        public static string GetHttpWebRequest(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(uri);
            myReq.UserAgent = "User-Agent:Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705";
            myReq.Accept = "*/*";
            myReq.KeepAlive = true;
            myReq.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            HttpWebResponse result = (HttpWebResponse)myReq.GetResponse();
            Stream receviceStream = result.GetResponseStream();
            StreamReader readerOfStream = new StreamReader(receviceStream, System.Text.Encoding.GetEncoding("utf-8"));
            string strHTML = readerOfStream.ReadToEnd();
            readerOfStream.Close();
            receviceStream.Close();
            result.Close();
            return strHTML;
        }
    }
}
