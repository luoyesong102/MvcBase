using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Esmart.Permission.Web
{
    /// <summary>
    /// WebRequest 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tpooo.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class WebRequest : System.Web.Services.WebService
    {

        [WebMethod]
        public string Request(string requestType, string requestbody)
        {
            return Esmart.Framework.Soa.SoaManager.InvorkWebService(requestType, requestbody);
        }
    }
}
