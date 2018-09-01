using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.Common;
using Esmart.Framework.Logging;
using Esmart.Framework.Model;
using Esmart.Framework;
using Esmart.Permission.Application;

namespace Esmart.Permission.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        UserLogInfo userInfo;

        List<string> functions;

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Esmart.Framework.Logging.LogManager.CreateLog4net().Error("applicationError", filterContext.Exception);//LOG4写文本
            UserNotLogInError(filterContext);//用户信息不对

            PageNotFindError(filterContext);//来源信息不对

            SerrorError(filterContext);//自定义异常

            SerrorUnKownError(filterContext);//处理为止异常
        }

        /// <summary>
        /// 登陆异常
        /// </summary>
        /// <param name="filterContext"></param>
        private void UserNotLogInError(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(UserNotLogInException))
            {
                //var url = "http://" + ConfigurationManager.AppSettings["SoaDomain"];
                var returnUrl = Request.Url.Host;
                if (Request.Url.Port != 80)
                {
                    returnUrl = "http://" + returnUrl + ":" + Request.Url.Port;
                }
                else
                {

                    returnUrl = "http://" + returnUrl;
                }
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Response.Write("<head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /> <title></title>");

                    Response.Write("<script> var thisobj = self.window;   var pageIndex = 0; while (!thisobj.TopSystemIndex) {  thisobj = thisobj.parent; pageIndex++;if (pageIndex > 5) { break;} }  thisobj.window.location ='/Account/LogIn?url=" + returnUrl + "'</script> ");
                    Response.Write("</head>");
                    Response.End();

                }
                else
                {
                    var json = new JsonReturn
                    {
                        Header =
                        {
                            Success = false,
                            Message = "<script> var thisobj = self.window;   var pageIndex = 0; while (!thisobj.TopSystemIndex) {  thisobj = thisobj.parent; pageIndex++;if (pageIndex > 5) { break;} }  thisobj.window.location ='/Account/LogIn?url=" +
                                returnUrl + "'</script> "
                        }
                    };
                    string jsonString = JsonConvert.SerializeObject(json);
                    Response.Write(jsonString);
                    Response.End();
                }
            }
        }

        /// <summary>
        /// 得到当前页面的功能
        /// </summary>
        public List<string> CurrentPageFunctions
        {
            get
            {
                if (functions != null)
                {
                    return functions;
                }
                var userId = CurrentUser.UserId;
                if (userId < 1) return new List<string>(0);
                var pageId = PageId;
                var response = new UserManager().GetFunctionByUserIdAndMenuId(userId, pageId);
                return functions = response.ConvertAll(a => a.FunctionKey);
            }
        }

        /// <summary>
        /// 页面没找到异常
        /// </summary>
        /// <param name="filterContext"></param>
        private void PageNotFindError(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(PageNotFindException))
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Response.Write("<head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /> <title></title>");
                    Response.Write("<script> var thisobj = self.window;   var pageIndex = 0; while (!thisobj.TopSystemIndex) {  thisobj = thisobj.parent; pageIndex++;if (pageIndex > 5) { break;} }  thisobj.window.location ='/Error/PageNotFind'</script> ");
                    Response.Write("</head>");
                    Response.End();

                }
                else
                {
                    var json = new JsonReturn
                    {
                        Header =
                        {
                            Success = false,
                            Message = "<script> var thisobj = self.window;   var pageIndex = 0; while (!thisobj.TopSystemIndex) {  thisobj = thisobj.parent; pageIndex++;if (pageIndex > 5) { break;} }  thisobj.window.location ='/Error/PageNotFind'</script> "
                        }
                    };
                    string jsonString = JsonConvert.SerializeObject(json);
                    Response.Write(jsonString);
                    Response.End();
                }
            }
        }

        /// <summary>
        /// 页面没找到异常
        /// </summary>
        /// <param name="filterContext"></param>
        private void SerrorError(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(TpoBaseException))
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">");
                    Response.Write("<head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /> <title></title>");
                    Response.Write("<script> var thisobj = self.window;   var pageIndex = 0; while (!thisobj.TopSystemIndex) {  thisobj = thisobj.parent; pageIndex++;if (pageIndex > 5) { break;} }  thisobj.window.location ='/Error/PageError'</script> ");
                    Response.Write("</head>");
                    Response.End();
                }
                else
                {
                    var json = new ResponseModel<string>
                    {
                        Header =
                        {
                            ReturnCode = 1,
                            Message = filterContext.Exception.Message
                        }
                    };
                    string jsonString = JsonConvert.SerializeObject(json);
                    Response.ContentType = "application/json";
                    Response.StatusCode = 200;
                    Response.Write(jsonString);
                    Response.End();
                }
            }
        }

        private void SerrorUnKownError(ExceptionContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\">");
                Response.Write("<head> <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /> <title></title>");
                Response.Write("<script> var thisobj = self.window;   var pageIndex = 0; while (!thisobj.TopSystemIndex) {  thisobj = thisobj.parent; pageIndex++;if (pageIndex > 5) { break;} }  thisobj.window.location ='/Account/LogIn'</script> ");
                Response.Write("</head>");
                Response.End();
            }
            else
            {
                Response.StatusCode = 500;
                Response.Write(filterContext.Exception.Message + "\n具体代码信息：" + filterContext.Exception.StackTrace);
                Response.End();
            }

        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var actionAuthorityIgnoreAttribute = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthorityIgnoreAttribute), true);

            var controllerAuthorityIgnoreAttribute = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthorityIgnoreAttribute), true);

            if (actionAuthorityIgnoreAttribute.Length != 0 || controllerAuthorityIgnoreAttribute.Length != 0)
            {
                //权限验证免了
                return;
            }

            if (CurrentUser != null)
            {
                ViewBag.Permission = CurrentPageFunctions;
            }
        }

        public UserLogInfo CurrentUser
        {
            get
            {
                return userInfo ?? (userInfo = new UserLogInfo { UserId = User.Identity.GetUserId<int>(), UserName = User.Identity.Name });
            }
        }

        /// <summary>
        /// 运用程序唯一ID
        /// </summary>
        public static int AppId
        {
            get
            {
                int appId;
                int.TryParse(GlobalConfig.AppID, out appId);
                return appId;
            }
        }

        /// <summary>
        /// 当前页面唯一ID
        /// </summary>
        public int PageId
        {
            get
            {
                int pageId;
                int.TryParse(Request.QueryString["PageId"], out pageId);
                return pageId;
            }
        }

        /// <summary>
        /// 验证的唯一Token
        /// </summary>
        public string Token
        {
            get
            {
                return Request.QueryString["Token"];
            }

        }
    }
}
