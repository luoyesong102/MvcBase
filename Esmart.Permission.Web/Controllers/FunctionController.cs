using System.Collections.Generic;
using System.Web.Mvc;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Web.Models.Common;
using Esmart.Framework.Model;
using Esmart.Permission.Application.Models.Permissions;

namespace Esmart.Permission.Web.Controllers
{
    public class FunctionController : BaseController
    {
        private readonly FunctionService _functionService;

        public FunctionController()
        {
            _functionService = new FunctionService();
        }

        [HttpPost]
        public PartialViewResult FunctionManagerDataTable(SoaDataPage<Function> filter)
        {
            CommonFunction.InitializeSoaDataPage(filter);

            var list = _functionService.GetFunctionsOfApp(filter);
            ViewBag.Count = list.Count;
            return PartialView(list.Body);
        }

        public PartialViewResult FunctionManagerFilter()
        {
            var list = new AppService().GetAppList();
            var AppList = new List<SelectListItem>();
            foreach (var item in list)
            {
                AppList.Add(new SelectListItem()
                {
                    Text = item.AppName,
                    Value = item.AppId.ToString()
                });
            }

            ViewBag.AppList = AppList;
            return PartialView();
        }

        public PartialViewResult FunctionManager()
        {
            var dataModel = new DataTableSource(0, "/Function/FunctionManagerDataTable", "/Function/FunctionManagerFilter");
            return PartialView(DataTableSource.TableViewName, dataModel);
        }

        [HttpGet]
        public PartialViewResult FunctionAdd(int AppId)
        {
            ViewBag.AppId = AppId;
            return PartialView();
        }

        [HttpPost]
        public JsonResult FunctionAdd(FunctionModel model)
        {
            ResponseModel<int> responser = new ResponseModel<int>();
            if (!_functionService.SaveFunction(model))
            {
                responser.Header.ReturnCode = 1;
                responser.Header.Message = "Fail";
            }
            return Json(responser);
        }

        public JsonResult FunctionDelete(int functionId)
        {

            var responser = new ResponseModel<int>();

            if (!_functionService.DeleteFunction(functionId))
            {
                responser.Header.ReturnCode = 1;
                responser.Header.Message = "Fail";
            }
            return Json(responser);
        }

        public PartialViewResult FunctionEdit(int functionId = 0)
        {
            var model = _functionService.GetFunction(functionId);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult FunctionEdit(FunctionModel model)
        {
            ResponseModel<int> responser = new ResponseModel<int>();
            if (!_functionService.SaveFunction(model))
            {
                responser.Header.ReturnCode = 1;
                responser.Header.Message = "Fail";
            }
            return Json(responser);
        }

        public JsonResult FunctionSearch()
        {
            return Json("");
        }
    }
}
