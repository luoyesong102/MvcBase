using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Esmart.Framework.Caching;
using Esmart.Framework.Logging;
using Esmart.Framework.Model;

namespace Esmart.Framework.Soa
{
    /// <summary>
    /// Soa服务调用
    /// </summary>
    public partial class SoaManager
    {

        /// <summary>
        /// 为一个接口创建对象
        /// </summary>
        /// <typeparam name="Tinterface">需要实现的接口</typeparam>
        /// <returns>返回实现后的类型</returns>
        public static Tinterface Invoke<Tinterface>()
        {
            return SoaCreateClass.CreateObj<Tinterface>();
        }

        /// <summary>
        /// 使用Expression动态编译程序
        /// </summary>
        /// <typeparam name="Tinterface">soa定义的接口类型</typeparam>
        /// <typeparam name="Treturn">返回类型</typeparam>
        /// <param name="express">lab表达式</param>
        /// <param name="processId">流程Id</param>
        /// <returns></returns>
        public static Esmart.Framework.Model.ResponseModel<Treturn> Invoke<Tinterface, Treturn>(System.Linq.Expressions.Expression<Func<Tinterface, Treturn>> express, string processId = null)
        {

            var body = express.Body as System.Linq.Expressions.MethodCallExpression;
            if (body == null)
            {
                throw new Esmart.Framework.Model.TpoBaseException("Tinterface类型为接口，必须是调用Tinterface的方法");
            }
            string requestType = string.Format("{0}.{1}", typeof(Tinterface).Name, body.Method.Name);

            object[] jsons = new object[body.Arguments.Count];

            for (int i = 0; i < body.Arguments.Count; i++)
            {
                var arg = body.Arguments[i];

                var constant = arg as System.Linq.Expressions.ConstantExpression;

                if (constant != null)
                {
                    jsons[i] = ConstantExpressionValue(constant, null);
                    continue;
                }
                var member = arg as System.Linq.Expressions.MemberExpression;

                if (member != null)
                {
                    jsons[i] = ConstantExpressionValue(member, member.Member.Name);
                    continue;
                }

                var listInitExpression = arg as System.Linq.Expressions.ListInitExpression;

                if (listInitExpression != null)
                {

                    jsons[i] = ConstantExpressionValue(listInitExpression);

                    continue;
                }

                var newArrayExpression = arg as System.Linq.Expressions.NewArrayExpression;

                if (newArrayExpression != null)
                {
                    jsons[i] = ConstantExpressionValue(newArrayExpression);
                    continue;
                }
            }
            var objs = typeof(Tinterface).GetCustomAttributes(typeof(ServerActionAttribute), false);
            if (objs.Length > 0)
            {
                var data = GetConfiguration(requestType);
                if (data != null)
                {
                    return InvokeServer<Treturn>(jsons, data);
                }
            }


            Model.RequestModel<object> requestModel = new Model.RequestModel<object>();

            requestModel.Header.RequestType = requestType;

            if (body.Arguments.Count == 1)
            {
                requestModel.Body = jsons[0];
            }
            if (body.Arguments.Count > 1)
            {
                requestModel.Body = jsons;
            }
            requestModel.Header.GUID = processId;

            return SOAClient.Request<object, Treturn>(requestModel);
        }


        internal static ResponseModel<Treturn> InvokeServer<Treturn>(object[] objs, ServerInfo server)
        {
            ResponseModel<Treturn> response = new ResponseModel<Treturn>();
            try
            {
                response.Body = (Treturn)server.Method.Invoke(server.InvokerObj, objs);
            }
            catch (Exception ex)
            {
                response.Header.ReturnCode = 1;
                response.Header.Message = ex.Message;
            }
            return response;
        }

        internal static object ConstantExpressionValue(ConstantExpression expression, string name)
        {
            if (expression != null)
            {
                if (expression.Value != null)
                {
                    var field = expression.Value.GetType().GetField(name);

                    if (field != null)
                    {
                        var value = field.GetValue(expression.Value);

                        return value;
                    }

                    if (expression.Value != null)
                    {
                        return expression.Value;
                    }
                }
            }
            return null;
        }


        internal static object ConstantExpressionObject(ConstantExpression expression, string name)
        {
            if (expression != null)
            {
                if (expression.Value != null)
                {
                    var field = expression.Value.GetType().GetField(name);

                    if (field != null)
                    {
                        var value = field.GetValue(expression.Value);

                        if (value != null)
                        {
                            return value;
                        }
                    }

                    return expression.Value;
                }
            }
            return null;
        }

        internal static object ConstantExpressionValue(MemberExpression expression, string name)
        {
            return ConstantExpressionValue(expression.Expression as ConstantExpression, name);
        }


        internal static object ConstantExpressionValue(ListInitExpression expression)
        {
            ReadOnlyCollection<ElementInit> elementInit = expression.Initializers;

            List<object> list = new List<object>();

            foreach (var element in elementInit)
            {
                var elementMemberExpression = element.Arguments[0] as MemberExpression;

                if (elementMemberExpression != null)
                {
                    var objectValue = ConstantExpressionObject(elementMemberExpression.Expression as ConstantExpression, elementMemberExpression.Member.Name);

                    if (objectValue != null)
                    {
                        list.Add(objectValue);
                    }
                }
            }
            return list;
        }



        internal static object ConstantExpressionValue(NewArrayExpression expression)
        {
            ReadOnlyCollection<Expression> elements = expression.Expressions;

            List<object> list = new List<object>();

            foreach (var ele in elements)
            {
                var memberExpression = ele as MemberExpression;

                if (memberExpression != null)
                {
                    var constant = memberExpression.Expression as ConstantExpression;

                    var objVvalue = ConstantExpressionObject(constant, memberExpression.Member.Name);

                    if (objVvalue != null)
                    {
                        list.Add(objVvalue);
                    }
                }
            }
            return list;
        }



    }



    public partial class SoaCreate<Treturn>
    {
        /// <summary>
        /// 使用Expression动态编译程序
        /// <typeparam name="Tinterface">soa定义的接口类型</typeparam>
        /// <param name="express">lab表达式</param>
        /// <returns></returns>
        /// </summary>
        public static Esmart.Framework.Model.ResponseModel<Treturn> Invoke<Tinterface>(System.Linq.Expressions.Expression<Func<Tinterface, Esmart.Framework.Model.ResponseModel<Treturn>>> express)
        {


            var body = express.Body as System.Linq.Expressions.MethodCallExpression;
            if (body == null)
            {
                throw new Esmart.Framework.Model.TpoBaseException("Tinterface类型为接口，必须是调用Tinterface的方法");
            }
            string requestType = string.Format("{0}.{1}", typeof(Tinterface).Name, body.Method.Name);

            object[] jsons = new object[body.Arguments.Count];

            for (int i = 0; i < body.Arguments.Count; i++)
            {
                var arg = body.Arguments[i];

                var constant = arg as System.Linq.Expressions.ConstantExpression;

                if (constant != null)
                {
                    jsons[i] = SoaManager.ConstantExpressionValue(constant, null);
                    continue;
                }
                var member = arg as System.Linq.Expressions.MemberExpression;

                if (member != null)
                {
                    jsons[i] = SoaManager.ConstantExpressionValue(member, member.Member.Name);
                    continue;
                }

                var listInitExpression = arg as System.Linq.Expressions.ListInitExpression;

                if (listInitExpression != null)
                {

                    jsons[i] = SoaManager.ConstantExpressionValue(listInitExpression);

                    continue;
                }

                var newArrayExpression = arg as System.Linq.Expressions.NewArrayExpression;

                if (newArrayExpression != null)
                {
                    jsons[i] = SoaManager.ConstantExpressionValue(newArrayExpression);
                    continue;
                }
            }
            var objs = typeof(Tinterface).GetCustomAttributes(typeof(ServerActionAttribute), false);
            if (objs.Length > 0)
            {
                var data = SoaManager.GetConfiguration(requestType);
                if (data != null)
                {
                    return SoaManager.InvokeServer<Treturn>(jsons, data);
                }
            }


            Model.RequestModel<object> requestModel = new Model.RequestModel<object>();

            requestModel.Header.RequestType = requestType;

            if (body.Arguments.Count == 1)
            {
                requestModel.Body = jsons[0];
            }
            if (body.Arguments.Count > 1)
            {
                requestModel.Body = jsons;
            }

            return SOAClient.Request<object, Treturn>(requestModel);
        }
    }



    public partial class SoaManager
    {

         static  object obj=new  object();

         private static Dictionary<string, ServerInfo> GetConfigurationAll()
         {
           //  NetCache catchs = (NetCache)Esmart.Framework.Caching.CacheManager.CreateNetCache();
             Dictionary<string, ServerInfo> dic = Esmart.Framework.Caching.CacheManager.CreateNetCache().Get<Dictionary<string, ServerInfo>>(typeof(Dictionary<string, ServerInfo>).FullName);
           
             if (dic != null && dic.Count > 0)
             {
                 return dic;
             }

             lock (obj)
             {
                 dic = new Dictionary<string, ServerInfo>();

                 var paths = AppDomain.CurrentDomain.BaseDirectory+"bin";

                 string[] files = System.IO.Directory.GetFiles(paths, @"*.dll", SearchOption.AllDirectories);

                 foreach (var file in files)
                 {
                     Assembly ass = null;
                     try
                     {
                         ass = Assembly.LoadFrom(file);
                     }
                     catch
                     {
                         continue;
                     }

                     if (ass == null)
                     {
                         continue;
                     }
                     var types = ass.GetTypes();

                     foreach (var type in types)
                     {
                         var serverActions = type.GetCustomAttributes(typeof(Esmart.Framework.Model.ServerActionAttribute), false);
                         if (serverActions.Count() > 0 && type.IsInterface)
                         {
                             var serverAction = (ServerActionAttribute)serverActions[0];

                             var dicChild = GetServerInfo(serverAction, type);

                             foreach (var key in dicChild)
                             {
                                 if (!dic.ContainsKey(key.Key.ToLower().Trim()))
                                 {
                                     dic.Add(key.Key.ToLower().Trim(), key.Value);
                                 }
                             }
                         }
                     }
                 }

                 Esmart.Framework.Caching.CacheManager.CreateNetCache().Add(typeof(Dictionary<string, ServerInfo>).Name, dic);
             }


             return dic;
         }


        public static ServerInfo GetConfiguration(string requestType)
        {
            ServerInfo model = null;

            var dic = GetConfigurationAll();

            dic.TryGetValue(requestType.ToLower().Trim(), out model);

            return model;
        }

        private static Dictionary<string, ServerInfo> GetServerInfo(ServerActionAttribute type, Type interfaceType)
        {
            Type typeaction = null;

            Dictionary<string, ServerInfo> dic = new Dictionary<string, ServerInfo>();

            if (type.ServerType.IsInterface)
            {
                throw new Esmart.Framework.Model.TpoBaseException("服务必须为实体类，不能为接口");
            }
            else
            {
                typeaction = type.ServerType;
            }

            var obj = Activator.CreateInstance(typeaction);
            var methods = interfaceType.GetMethods();
            var typename = interfaceType.Name;
            foreach (var method in methods)
            {
                ServerInfo model = new ServerInfo();

                if (method.IsGenericMethod)
                {
                    var methodAll = typeaction.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                    var data = methodAll.Where(a => a.Name == method.Name && a.GetGenericArguments().Length == method.GetGenericArguments().Length);

                    if (data.Count() != 1)
                    {
                        throw new Esmart.Framework.Model.TpoBaseException("方法名称同名 ，方法名称是" + method.Name);
                    }
                    model.Method = data.First();
                }
                else
                {
                    var pars = method.GetParameters();

                    List<Type> list = new List<Type>();

                    foreach (var par in pars)
                    {
                        list.Add(par.ParameterType);
                    }
                    model.Method = typeaction.GetMethod(method.Name, list.ToArray());
                }
                if (!model.Method.IsStatic)
                {
                    model.InvokerObj = obj;
                }
                string key = string.Format("{0}.{1}", typename, method.Name);
                if (dic.ContainsKey(key))
                {
                    throw new Esmart.Framework.Model.TpoBaseException("服务:" + key + ",已经提供，不能在提供此服务");
                }
                else
                {
                    dic.Add(key, model);
                }

            }
            return dic;
        }



        private static object InvokeAction(string requestType, string requestbody)
        {

            if (string.IsNullOrEmpty(requestType))
            {
                throw new Exception("requestType不能为空");
            }
            object returnobj = null;

            ServerInfo requestinfo = GetConfiguration(requestType);


            if (requestinfo == null)
            {
                throw new Exception("没提供此服务，请联系相关开发人员");
            }


            MethodInfo method = requestinfo.Method;

            if (method == null)
            {
                throw new Exception("开发人员配置信息错误");
            }

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length == 0)
            {
                returnobj = method.Invoke(requestinfo.InvokerObj, null);
            }
            else if (parameters.Length == 1)
            {
                Type typePar = parameters[0].ParameterType;

                if (typePar == typeof(string))
                {
                    returnobj = method.Invoke(requestinfo.InvokerObj, new object[] { requestbody });
                }
                else
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(requestbody, typePar);

                    returnobj = method.Invoke(requestinfo.InvokerObj, new object[] { obj });

                }
            }
            else
            {
                object[] objs = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(requestbody);

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != typeof(string))
                    {
                        objs[i] = Newtonsoft.Json.JsonConvert.DeserializeObject(objs[i].ToString(), parameters[i].ParameterType);
                    }
                }
                returnobj = method.Invoke(requestinfo.InvokerObj, objs);
            }
            return returnobj;
        }


        private static object InvokeAction(string requestType, HttpApplication content)
        {
            var server = Esmart.Framework.Soa.SoaManager.GetConfiguration(requestType);

            if (server == null)
            {
                return null;
            }
            var parameter = server.Method.GetParameters();

            object[] objs = new object[parameter.Length];

            for (int i = 0; i < parameter.Length; i++)
            {
                if (parameter[i].ParameterType.IsGenericType)
                {
                    if (parameter[i].ParameterType.IsValueType)
                    {
                        var value = content.Request.QueryString[parameter[i].Name];

                        if (value != null)
                        {
                            var getGenericArguments = parameter[i].ParameterType.GetGenericArguments();


                            objs[i] = Convert.ChangeType(value, getGenericArguments[0]);
                        }
                    }
                    else
                    {
                        objs[i] = Esmart.Framework.Model.CommonFunction.InitJqueryAjaxRequest(parameter[i].ParameterType);
                    }


                }
                else
                {
                    if (parameter[i].ParameterType.IsValueType)
                    {
                        var value = content.Request.QueryString[parameter[i].Name];
                        if (value == null)
                        {
                            throw new Esmart.Framework.Model.TpoBaseException(parameter[i].Name + "不能为空");
                        }

                        if (parameter[i].ParameterType.IsEnum)
                        {
                            int enumResult = 0;
                            if (int.TryParse(value, out enumResult))
                            {
                                var name = Enum.GetName(parameter[i].ParameterType, enumResult);

                                if (string.IsNullOrEmpty(name))
                                {
                                    throw new Esmart.Framework.Model.TpoBaseException(parameter[i].Name + "类型转换失败");
                                }
                                objs[i] = Enum.Parse(parameter[i].ParameterType, name);

                            }
                            else
                            {
                                objs[i] = Enum.Parse(parameter[i].ParameterType, value);
                            }

                            continue;
                        }
                        try
                        {
                            objs[i] = Convert.ChangeType(value, parameter[i].ParameterType);
                        }
                        catch
                        {
                            throw new Esmart.Framework.Model.TpoBaseException(parameter[i].Name + "类型转换失败");
                        }

                    }
                    else if (parameter[i].ParameterType == typeof(string))
                    {
                        objs[i] = content.Request.QueryString[parameter[i].Name];
                    }
                    else
                    {
                        objs[i] = Esmart.Framework.Model.CommonFunction.InitJqueryAjaxRequest(parameter[i].ParameterType);
                    }
                }
            }

            var returnObj = server.Method.Invoke(server.InvokerObj, objs);

            return returnObj;
        }

        /// <summary>
        /// 仿照wcf执行WebService服务
        /// </summary>
        /// <param name="requestType">访问唯一Id</param>
        /// <param name="requestbody">访问参数</param>
        /// <returns></returns>
        public static string InvorkWebService(string requestType, string requestbody)
        {
            return InvorkWebService(requestType, requestbody, null);
        }

        /// <summary>
        /// 执行url重写的信息
        /// </summary>
        /// <returns></returns>
        public static string InvorkWebService(string requestType, HttpApplication content)
        {
            return InvorkWebService(requestType, null, content);
        }


        private static string InvorkWebService(string requestType, string requestbody, HttpApplication content)
        {
            ResponseModel<object> response = new ResponseModel<object>();
            try
            {
                using (new Trace(requestType, 2))
                {
                    if (content == null)
                    {
                        response.Body = InvokeAction(requestType.Trim(), requestbody);
                    }
                    //else
                    //{
                    //    response.Body = InvokeAction(requestType, content);
                    //}
                }
            }
            catch (Exception ex)
            {
                string message = "系统出现未知异常，请稍后重试！";
                response.Header.ReturnCode = 1;//服务端错误
                if (ex.InnerException != null)
                {
                    var inner = ex.InnerException;
                    if (inner.InnerException != null)
                    {
                        ex = inner.InnerException;
                    }
                    var baseException = ex.InnerException as Esmart.Framework.Model.TpoBaseException;
                    if (baseException != null)
                    {
                        response.Header.ReturnCode = baseException.Code;
                        response.Header.Message = ex.InnerException.Message;
                    }
                    else
                    {
                        if (ConstantDefine.SoaDebug)
                        {
                            response.Header.Message = message + ex.InnerException.Message;
                        }
                        else
                        {
                            response.Header.Message = message;
                        }
                      
                    }

                    Esmart.Framework.Logging.LogManager.CreateTpoLog().Error("服务出现异常", ex.InnerException);
                }
                else
                {
                    if (ConstantDefine.SoaDebug)
                    {
                        response.Header.Message = message + ex.Message;
                    }
                    else
                    {
                        response.Header.Message = message;
                    }
                    Esmart.Framework.Logging.LogManager.CreateTpoLog().Error("服务出现异常", ex);
                }

            }

            string  returnJson=  Newtonsoft.Json.JsonConvert.SerializeObject(response);

            if (content != null)
            {
                if (content.Request.QueryString["CallBack"] != null)
                {

                    string jsAction = "eval(\"(" + returnJson + ")\")";
                    content.Response.Write(string.Format("{0}({1})", content.Request.QueryString["CallBack"], returnJson));
                }
                else
                {
                    content.Response.Write(returnJson);
                }
                content.Response.Flush();

                content.Response.End();
            }
            else
            {
                return returnJson;
            }
            return returnJson;
        }
    }


     public class ServerInfo
     {
         public System.Reflection.MethodInfo Method { get; set; }

         /// <summary>
         /// 被调用的实体对象
         /// </summary>
         public object InvokerObj { get; set; }

     }
}
