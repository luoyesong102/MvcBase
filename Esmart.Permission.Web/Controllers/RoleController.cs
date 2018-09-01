using System;
using System.Web.Mvc;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Web.Models.Common;
using Esmart.Permission.Web.Models.Permissions;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web.Controllers
{
    public class RoleController : BaseController
    {
        private readonly RoleService _service;

        public RoleController()
        {
            _service = new RoleService();
        }

        public ViewResult Index()
        {
            var dataSource = new DataTableSource(
                0,
                Url.Action("RoleGrid", "Role"),
                Url.Action("RoleGridFilter", "Role")
                );
            return View(DataTableSource.TableViewName, dataSource);
        }

        public PartialViewResult RoleList(int departmentId = 0)
        {
            var dataSource = new DataTableSource(
                0,
                Url.Action("RoleGrid", "Role"),
                Url.Action("RoleGridFilter", "Role", new { departId = departmentId })
                );
            return PartialView(DataTableSource.TableViewName, dataSource);
        }

        [HttpGet]
        public PartialViewResult RoleGridFilter()
        {
            return PartialView();
        }

        [HttpPost]
        public PartialViewResult RoleGrid(SoaDataPage<RoleGridFilterViewModel> filter)
        {
            CommonFunction.InitializeSoaDataPage(filter);

            filter.Where.LogInUserId = CurrentUser.UserId;

            var rolesList = _service.GetDepartmentRoles(filter);

            return PartialView(rolesList);
        }

        [HttpGet]
        public PartialViewResult AddRole()
        {

            return PartialView("EditRole");
        }

        [HttpGet]
        public PartialViewResult EditRole(int roleId)
        {
            var role = _service.GetRole(roleId);
            return PartialView(role);
        }

        [HttpGet]
        public PartialViewResult EditRolePermission(int roleId)
        {
            var rolePermissions = new RolePermissionsViewModel(roleId, CurrentUser.UserId);
            return PartialView(rolePermissions);
        }

        public string GetRolePermissionTreeJson(int roleId, int appId)
        {
            var roleTree = new RolePermissionsTreeViewModel(roleId, CurrentUser.UserId, appId);
            return roleTree.TreeNodesJson;
        }

        [HttpPost]
        public JsonResult SaveRolePermission(RolePermissionsRequestModel model)
        {
            model.CreatorId = CurrentUser.UserId;
            model.CreateTime = DateTime.Now;
            model.UpdaterId = CurrentUser.UserId;
            model.UpdateTime = DateTime.Now;
            new PermissionService().AssignRolePermission(model);
            var result = new ResponseModel<string>()
            {
                Body = "保存成功！"
            };
            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveRole(RoleModel role)
        {
            role.CreatorId = CurrentUser.UserId;
            role.CreateTime = DateTime.Now;
            _service.SaveRole(role);
            var result = new ResponseModel<string>()
            {
                Body = "保存成功！"
            };
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteRole(int roleId)
        {
            _service.DeleteRole(roleId);
            var result = new ResponseModel<string>()
            {
                Body = "保存成功！"
            };
            return Json(result);
        }
    }
}
