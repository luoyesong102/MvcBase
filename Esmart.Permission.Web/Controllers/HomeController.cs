using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Esmart.Framework.Caching;
using Esmart.Framework.Model;
using Esmart.Framework;
using Esmart.Permission.Application;

namespace Esmart.Permission.Web.Controllers
{
    public class HomeController : BaseController
    {
        [AuthorityIgnore]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId<int>();

            ViewBag.CurrentUserId = User.Identity.GetUserId<int>();

            var apps = new UserManager().GetAppListByUserId(userId);

            var token = "?Token=" + Uri.EscapeDataString(CommonFunction.Encrypt(userId.ToString()));

            ViewBag.Apps = apps.Select(app => new SelectListItem
            {
                Text = app.AppName,
                Value = "http://" + app.Domain + token,
                Selected = app.AppId.ToString() == GlobalConfig.AppID
            }).ToList();

            var menus = new UserManager().GetMenuByUserId(userId, AppId);

            return View(menus);
        }

        public JsonResult ClearCache()
        {
            try
            {
                CacheManager.CreateCache().FlushAll();
                CacheManager.CreateRedisCache().FlushAll();
                return Json(new { message = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = "error:" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
