using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Xml.Serialization;
using Esmart.Framework.Caching;
using Esmart.Framework.Soa;


namespace Esmart.Framework.Model
{
    /// <summary>
    /// 公共方法提供
    /// </summary>
    public partial class CommonFunction
    {
        private static long localId = 0;

        static CommonFunction()
        {
            //localId = GetGuid();
        }

        /// <summary>
        /// 得到唯一的ID
        /// </summary>
        /// <returns></returns>
        public static Int64 GetGuid()
        {
            Model.RequestModel<object> model = new Model.RequestModel<object>();
            model.Header.RequestType = "Esmart.Guid.NewGuid";


            Model.ResponseModel<Int64> reponse = SOAClient.Request<object, Int64>(model);

            if (reponse.Header.ReturnCode == 0)
            {
                return reponse.Body;
            }

            return DateTime.Now.Ticks;
        }


        /// <summary>
        /// 得到本地的唯一ID，不推荐
        /// </summary>
        /// <returns></returns>
        public static Int64 GetLocalGuid()
        {
            Interlocked.Increment(ref localId);
            return localId;
        }

        /// <summary>
        /// 装换成MD5类型
        /// </summary>
        /// <param name="originalString"></param>
        /// <returns></returns>
        public static string GetMD5String(string originalString)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(originalString, "MD5").Substring(0, 20);
        }
        public static string GetMD5String32(string timestamp, string appSecret)
        {
            string originalString = timestamp + "&" + appSecret;

            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(originalString.ToLower(), "MD5").ToLower();
        }
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        private static string tickKey = "LogIn_Ticker_Key";



        public static string Encrypt(string encryptString)
        {
            try
            {

                byte[] rgbKey = Encoding.UTF8.GetBytes(tickKey.Substring(0, 8));
                byte[] rgbIV = rgbKey;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                return Convert.ToBase64String(mStream.ToArray());

            }
            catch
            {
                return string.Empty;
            }


        }



        public static string Decrypt(string decryptString)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(tickKey.Substring(0, 8));
                byte[] rgbIV = rgbKey;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                cStream.Close();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 得到提交的ajax信息
        /// </summary>
        /// <typeparam name="Tobj"></typeparam>
        /// <returns></returns>
        public static void RegisSubMit<Tobj>()
        {
            StringBuilder strBulid = new StringBuilder();
            Type type = typeof(Tobj);
            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in proertyinfos)
            {
                if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface) && info.PropertyType != typeof(string))
                {
                    continue;
                }
                if (strBulid.Length == 0)
                {
                    strBulid.Append(info.Name + ":$('#" + info.Name + "').val()");
                }
                else
                {
                    strBulid.Append("," + info.Name + ":$('#" + info.Name + "').val()");
                }
            }
            System.Web.HttpContext.Current.Response.Write(" <script type=\"text/javascript\">  function getSubMitDataInfo() { return { " + strBulid.ToString() + " } } </script>");
        }


        /// <summary>
        /// MVC对SoaDataPage内容设置数值
        /// </summary>
        /// <typeparam name="Tmodel"></typeparam>
        /// <param name="soaDataPage"></param>
        public static void InitializeSoaDataPage<Tmodel>(SoaDataPage<Tmodel> soaDataPage)
        {
            Type type = typeof(Tmodel);
            if (type.IsClass && type != typeof(string))
            {
                Tmodel model = Activator.CreateInstance<Tmodel>();
                PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo info in proertyinfos)
                {
                    if ((info.PropertyType.IsClass || info.PropertyType.IsAbstract || info.PropertyType.IsInterface) && info.PropertyType != typeof(string))
                    {
                        continue;
                    }
                    var value = System.Web.HttpContext.Current.Request["Where[" + info.Name + "]"];

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }


                    var propertype = info.PropertyType;

                    if (info.PropertyType.IsGenericType)
                    {
                        var types = info.PropertyType.GetGenericArguments();
                        propertype = types[0];
                    }
                    if (propertype.IsEnum)
                    {
                        dynamic obj = Enum.Parse(propertype, value);
                        info.SetValue(model, obj, null);
                    }
                    else
                    {
                        dynamic obj = Convert.ChangeType(value, propertype);

                        info.SetValue(model, obj, null);
                    }



                }
                soaDataPage.Where = model;
            }
        }



        /// <summary>
        /// juqeryajax异步访问的时候，对传递过来的对象做处理
        /// </summary>
        /// <typeparam name="Tmodel">实例化的类型></typeparam>
        public static Tmodel InitJqueryAjaxRequest<Tmodel>() where Tmodel : class
        {
            Tmodel model = Activator.CreateInstance<Tmodel>();

            string[] keys = System.Web.HttpContext.Current.Request.Form.AllKeys;

            SetValue(model, null, keys);

            return model;
        }



        /// <summary>
        /// juqeryajax异步访问的时候，对传递过来的对象做处理
        /// </summary>
        public static object InitJqueryAjaxRequest(Type type)
        {
            object model = Activator.CreateInstance(type);


            string[] keys = System.Web.HttpContext.Current.Request.Form.AllKeys;


            string[] queryAllKeys = System.Web.HttpContext.Current.Request.QueryString.AllKeys;


            List<string> list = new List<string>(keys.Length + queryAllKeys.Length);

            list.AddRange(keys);

            list.AddRange(queryAllKeys);

            SetValue(model, null, list.ToArray());

            return model;
        }



        private static void SetValue(object obj, string key, string[] keys)
        {
            string findKey = null;

            var properinfos = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var properinfo in properinfos)
            {
                if (string.IsNullOrEmpty(key))
                {
                    findKey = properinfo.Name;
                }
                else
                {
                    findKey = key + "[" + properinfo.Name + "]";
                }

                if (properinfo.PropertyType.IsValueType || properinfo.PropertyType == typeof(string))
                {
                    SetValueTypeOrString(properinfo, obj, findKey);
                    continue;
                }

                Type iEnumerable = properinfo.PropertyType.GetInterface("IEnumerable", false);

                if (iEnumerable != null)
                {
                    if (properinfo.PropertyType.IsGenericType)
                    {
                        var arguments = properinfo.PropertyType.GetGenericArguments();

                        if (arguments[0].IsValueType || arguments[0] == typeof(string))
                        {
                            SetIEnumerableValueType(arguments[0], properinfo, obj, findKey);
                            continue;
                        }
                        else
                        {
                            SetIEnumerableClass(properinfo, obj, findKey, keys);
                            continue;
                        }
                    }
                }

                if (properinfo.PropertyType.IsClass && iEnumerable == null)
                {
                    dynamic model = Activator.CreateInstance(properinfo.PropertyType);

                    if (IsExistesForm(model, findKey, keys))
                    {
                        SetValue(model, findKey, keys);

                        properinfo.SetValue(obj, model, null);
                    }
                }
            }
        }


        /// <summary>
        /// 设置IEnumerable结果
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="properinfo"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        private static void SetIEnumerableValueType(Type argument, System.Reflection.PropertyInfo properinfo, object obj, string key)
        {

            dynamic dynList = Activator.CreateInstance(properinfo.PropertyType);

            string findkey = key + "[]";

            string findValue = System.Web.HttpContext.Current.Request[findkey];

            if (string.IsNullOrEmpty(findValue))
            {
                return;
            }

            string[] strArray = findValue.Split(',');

            foreach (string stra in strArray)
            {
                if (!string.IsNullOrEmpty(stra))
                {
                    dynamic strasas = Convert.ChangeType(stra, argument);
                    dynList.Add(strasas);
                }
            }

            properinfo.SetValue(obj, dynList, null);
        }


        /// <summary>
        /// 对值类型设置初始值
        /// </summary>
        /// <param name="properinfo"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        private static void SetValueTypeOrString(System.Reflection.PropertyInfo properinfo, object obj, string key)
        {
            string findKey = System.Web.HttpContext.Current.Request[key];

            if (string.IsNullOrEmpty(findKey))
            {
                return;
            }

            var propertyType = properinfo.PropertyType;
            if (propertyType.IsGenericType)
            {
                propertyType = properinfo.PropertyType.GetGenericArguments()[0];
            }

            if (propertyType.IsEnum)
            {
                dynamic value = Enum.Parse(propertyType, findKey);
                properinfo.SetValue(obj, value, null);
            }
            else
            {
                dynamic value = Convert.ChangeType(findKey, propertyType);
                properinfo.SetValue(obj, value, null);
            }

        }


        /// <summary>
        /// 对IEnumerable设置初始值
        /// </summary>
        /// <param name="properinfo"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="keys"></param>
        private static void SetIEnumerableClass(System.Reflection.PropertyInfo properinfo, object obj, string key, string[] keys)
        {

            var arguments = properinfo.PropertyType.GetGenericArguments();

            var properinfos = arguments[0].GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            dynamic objList = Activator.CreateInstance(properinfo.PropertyType);

            string findkey = null;

            for (int i = 0; i < keys.Length; i++)
            {
                findkey = key + "[" + i + "]";

                bool isExistes = false;

                foreach (var info in properinfos)
                {
                    string findlistkey = findkey + "[" + info.Name + "]";

                    findlistkey = keys.FirstOrDefault(a => a.ToLower().Trim() == findlistkey.ToLower().Trim());

                    if (string.IsNullOrEmpty(findlistkey))
                    {
                        continue;
                    }

                    string findvalue = System.Web.HttpContext.Current.Request[findlistkey];

                    if (string.IsNullOrEmpty(findvalue))
                    {
                        continue;
                    }

                    isExistes = true;

                    break;
                }

                if (isExistes)
                {
                    dynamic objvalue = Activator.CreateInstance(arguments[0]);

                    SetValue(objvalue, findkey, keys);

                    objList.Add(objvalue);
                }

            }
            properinfo.SetValue(obj, objList, null);
        }


        private static bool IsExistesForm(object obj, string key, string[] keys)
        {
            string findKey = null;
            var properinfos = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            bool isExistes = false;
            foreach (var properinfo in properinfos)
            {
                if (string.IsNullOrEmpty(key))
                {
                    findKey = properinfo.Name;
                }
                else
                {
                    findKey = key + "[" + properinfo.Name + "]";
                }

                if (keys.FirstOrDefault(a => a.Trim().ToLower() == findKey.Trim().ToLower()) != null)
                {
                    isExistes = true;
                    break;
                }
            }

            return isExistes;
        }


        /// <summary>
        /// 得到客户端的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientUserIP()
        {
            try
            {
                string result = String.Empty;

                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                // 如果使用代理，获取真实IP 
                if (result != null && result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
                    result = null;
                else if (result != null)
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有“,”，估计多个代理。取第一个不是内网的IP。 
                        result = result.Replace(" ", "").Replace("'", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i])
                                && temparyip[i].Substring(0, 3) != "10."
                                && temparyip[i].Substring(0, 7) != "192.168"
                                && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i];    //找到不是内网的地址 
                            }
                        }
                    }
                    else if (IsIPAddress(result)) //代理即是IP格式 
                        return result;
                    else
                        result = null;    //代理中的内容 非IP，取IP 
                }
                if (null == result || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (result == null || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.UserHostAddress;

                return result;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 得到服务器名字
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            return System.Net.Dns.GetHostName();
        }



        /// <summary>
        /// 判断是否是IP地址格式 0.0.0.0
        /// </summary>
        /// <param name="str1">待判断的IP地址</param>
        /// <returns>true or false</returns>
        private static bool IsIPAddress(string str1)
        {
            if (str1 == null || str1 == string.Empty || str1.Length < 7 || str1.Length > 15) return false;

            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);

            return regex.IsMatch(str1);
        }



        private static Dictionary<string, SqlContent> GetSqlDocuments()
        {

            Dictionary<string, SqlContent> dic = System.Web.HttpContext.Current.Cache.Get(typeof(Dictionary<string, SqlContent>).FullName) as Dictionary<string, SqlContent>;

            if (dic != null && dic.Count > 0)
            {
                return dic;
            }

            string parentPath = AppDomain.CurrentDomain.BaseDirectory;

            dic = new Dictionary<string, SqlContent>();

            XmlSerializer _sqlDocSerializer = new XmlSerializer(typeof(SqlDocument));

            string[] files = System.IO.Directory.GetFiles(parentPath, @"*.xml", SearchOption.AllDirectories);

            files = files.Where(a => a.IndexOf("\\XmlConfig\\", StringComparison.OrdinalIgnoreCase) != -1).ToArray();

            System.Web.Caching.CacheDependency dependency = new System.Web.Caching.CacheDependency(files);

            foreach (var file in files)
            {
                using (var stream = File.OpenRead(file))
                {
                    try
                    {
                        var doc = (SqlDocument)_sqlDocSerializer.Deserialize(stream);

                        if (doc != null && doc.Entities != null)
                        {
                            foreach (var model in doc.Entities)
                            {
                                if (!dic.ContainsKey(model.Name))
                                {
                                    dic.Add(model.Name, new SqlContent() { Content = model.Content, Parameter = model.Parameter });
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Esmart.Framework.Logging.LogManager.CreateTpoLog().Error("sqlxml基础信息读取失败", ex);
                    }
                }
            }
            HttpContext.Current.Cache.Add(typeof(Dictionary<string, SqlContent>).FullName, dic, dependency, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null);

            return dic;
        }


        /// <summary>
        /// 根据唯一name获取sql信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SqlContent GetSqlStringByName(string name)
        {
            SqlContent content = null;

            var dic = GetSqlDocuments();

            dic.TryGetValue(name, out content);

            return content;
        }

    }

    /// <summary>
    /// 验证所有的
    /// </summary>
    public partial class CommonFunction
    {
        public static bool ValidAll(object model, ref string message)
        {
            List<string> listStr = new List<string>();

            Type type = model.GetType();

            PropertyInfo[] proertyinfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo info in proertyinfos)
            {
                var attributes = info.GetCustomAttributes(typeof(VerificationAttribute), false);

                if (attributes.Count() == 0)
                {
                    continue;
                }

                var value = info.GetValue(model, null);

                foreach (var att in attributes)
                {
                    var valid = att as VerificationAttribute;

                    if (!valid.IsAllowEmpty && value == null)
                    {
                        listStr.Add(valid.Message);
                        break;
                    }

                    if (value == null)
                    {
                        break;
                    }

                    if (valid.MaxSize > 0)
                    {
                        var ismax = value.ToString().Length <= valid.MaxSize && value.ToString().Length >= valid.MinSize;
                        if (!ismax)
                        {
                            listStr.Add(valid.Message);
                            continue;
                        }
                    }
                    if (!string.IsNullOrEmpty(valid.RegularString))
                    {
                        var flag = System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), valid.RegularString);

                        if (!flag)
                        {
                            listStr.Add(valid.Message);
                        }
                    }
                }

            }
            if (listStr.Count > 0)
            {
                message = string.Join("，", listStr);
                return false;
            }
            return true;
        }



    }

    /// <summary>
    /// 验证逻辑
    /// </summary>
    public class VerificationConstantDefine
    {
        /// <summary>
        ///  非空支付串
        /// </summary>
        public const string NonEmptyPatten = @"^\s*$";




        /// <summary>
        /// 验证汉字
        /// </summary>
        public const string ChinesePatten = @"^[\u4e00-\u9fa5]{0,}$";

        /// <summary>
        /// 数字验证
        /// </summary>
        public const string NumberPatten = @"^[-]?[0-9]*$";

        /// <summary>
        /// 正整数验证
        /// </summary>
        public const string PositiveIntegerPatten = @"^[1-9]?[0-9]*$";

        /// <summary>
        /// 邮件验证
        /// </summary>
        public const string EmailPatten = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        /// 手机号码验证
        /// </summary>
        public const string TelPhonelPatten = @"^([0\+]\d{2,3}-)?(\d{11})$";


        /// <summary>
        /// 时间验证格式(yyyy-MM-dd  HH:mm:ss)或者是yyyy/MM/dd  HH:mm:ss
        /// </summary>
        public const string DateTimePatten = @"^[1-9][0-9]{3}[-//][0-9]{2}[-//][0-9]{2}\s+[0-9]{2}:[0-9]{2}:[0-9]{2}$";

        /// <summary>
        /// 日期验证(yyyy-MM-dd)或者是yyyy/MM/dd
        /// </summary>
        public const string DatePatten = @"^[1-9][0-9]{3}[-//][0-9]{2}[-//][0-9]{2}";


        /// <summary>
        /// Url验证
        /// </summary>
        public const string UrlPatten = @"^http|https:\S+\.{1}\S+$";

        /// <summary>
        /// 英语验证
        /// </summary>
        public const string EnglisPatten = @"^[A-Za-z]+$";


        /// <summary>
        /// double验证
        /// </summary>
        public const string DoublePatten = @"^[-]?\d+(\.\d+)?$";

    }


    public class RegisModule : IHttpModule
    {

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest +=
                (new EventHandler(this.Context_BeginRequest));
        }

        public void Context_BeginRequest(object sender, EventArgs e)
        {
            var content = (HttpApplication)sender;

            var absolutePath = content.Request.Url.AbsolutePath;

            if (string.IsNullOrEmpty(absolutePath))
            {
                return;
            }

            if (absolutePath == "/")
            {
                return;
            }


            var segments = content.Request.Url.Segments;

            if (segments.Length < 2)
            {
                return;
            }

            if (absolutePath.IndexOf(".asmx", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return;
            }




            if (segments.Length > 2)
            {
                var requestType = string.Format("{0}.{1}", segments[1].Replace('/', ' ').Trim(), segments[2].Replace('/', ' ').Trim());

                SoaManager.InvorkWebService(requestType, content);
            }

        }
    }
}
