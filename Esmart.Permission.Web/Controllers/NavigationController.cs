using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Esmart.Permission.Application.Models.Common;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web.Controllers
{
    public class NavigationController : BaseController
    {
        private readonly MenuService _menuService;

        public NavigationController()
        {
            _menuService = new MenuService();
        }

        public ActionResult NavigationManager(int appId = 0)
        {
            ViewBag.addStr = "";

            var list = new AppService().GetAppList();
            var AppList = new List<SelectListItem>();
            foreach (var item in list)
            {
                var addItem = new SelectListItem()
                {
                    Text = item.AppName,
                    Value = item.AppId.ToString()
                };
                if (item.AppId == appId)
                {
                    addItem.Selected = true;
                }
                AppList.Add(addItem);
            }
            ViewBag.AppList = AppList;
            return View();
        }

        [HttpGet]
        public PartialViewResult NavigationAdd(int appId = 0, int parentNavigationId = 0)
        {
            ViewBag.ParentNavigationId = parentNavigationId;
            ViewBag.AppId = appId;
            return PartialView();
        }

        public JsonResult NavigationAdd(MeunModel model)
        {
            var responser = new ResponseModel<MeunModel>();
            model.CreateId = CurrentUser.UserId;
            model.CreateTime = DateTime.Now;
            var Id = _menuService.NavigationSave(model);
            model.Id = Id;
            if (Id <= 0)
            {
                responser.Header.ReturnCode = 1;
                responser.Header.Message = "Fail";
            }
            responser.Body = model;
            return Json(responser);
        }

        public JsonResult NavigationDelete(int navigationId = 0)
        {
            var responser = new ResponseModel<MeunModel>();
            _menuService.DeleteAll(navigationId);
            return Json(responser);
        }

        [HttpGet]
        public PartialViewResult NavigationEdit(int navigationId = 0)
        {
            var data = _menuService.GetMenusByNavigationId(navigationId);
            return PartialView(data);
        }

        public JsonResult NavigationEdit(MeunModel model)
        {
            var responser = new ResponseModel<MeunModel>();
            model.Id = _menuService.NavigationSave(model);
            if (model.Id <= 0)
            {
                responser.Header.ReturnCode = 1;
                responser.Header.Message = "Fail";
            }
            responser.Body = model;
            return Json(responser);
        }

        /// <summary>
        /// 分配功能
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PartialViewResult NavigationFunction(int appId = 0, int navigationId = 0)
        {
            var list = _menuService.GetAllAndChoice(appId, navigationId);
            return PartialView(list);
        }

        public JsonResult NavigationFunction(NavigationFunctionRequest model)
        {
            var responser = new ResponseModel<MeunModel>();
            model = CommonFunction.InitJqueryAjaxRequest<NavigationFunctionRequest>();
            model.CreatId = CurrentUser.UserId;
            _menuService.NavigationFunction(model);
            return Json(responser);
        }

        public ActionResult NavigationTree(int appId)
        {
            var list = _menuService.GetNavigationTreeData(appId);
            var model = new ResponseModel<List<ZTreeNodeJson>> {Body = list};
            return Json(model);
        }
    }
}
