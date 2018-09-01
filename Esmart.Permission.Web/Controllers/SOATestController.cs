using System.Web.Mvc;

namespace Esmart.Permission.Web.Controllers
{
    public class SOATestController : Controller
    {
        public ActionResult Index()
        {
            var result = new Permission.Application.RoleService().GetUsersByFunctionKey("AppyScheduling", 2264);
            return View();
        }
    }
}