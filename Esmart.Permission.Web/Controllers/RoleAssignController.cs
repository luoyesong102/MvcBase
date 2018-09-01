using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web.Controllers
{
    public class RoleAssignController : BaseController
    {
        public ActionResult Index()
        {
            var departments = new DepartmentService().GetDepartments(CurrentUser.UserId, 1);

            if (departments.Count > 0)
            {
                departments[0].Open = true;
            }
            else
            {
                return View("SimpleMessage", new ModelError("当前用户没有所属部门!"));
            }

            ViewBag.ZTreeNodes = JsonConvert.SerializeObject(departments, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            return View();
        }

        public PartialViewResult MainContentPart(int departmentId = 0, int userId = 0)
        {
            ViewBag.userId = userId;
            ViewBag.departmentId = departmentId;
            return PartialView(departmentId);
        }

        public PartialViewResult SearchPart()
        {
            return PartialView();
        }

        public PartialViewResult TablePart(int departmentId = 0, int userId = 0)
        {
            var model = new RoleService().GetAssignRoles(CurrentUser.UserId, departmentId, userId);
            return PartialView((model ?? new List<RoleResponse>(0)).OrderByDescending(n => n.IsChoice));
        }

        /// <summary>
        /// 保存角色分配
        /// </summary>
        public JsonResult SaveRoles(int departmentId, int userId, string roleIds)
        {
            var ids = string.IsNullOrWhiteSpace(roleIds) ? new List<int>(0) : roleIds.Split(',').Select(int.Parse).ToList();

            if (departmentId > 0)
            {
                var result = new ResponseModel<int>
                {
                    Body = new RoleService().UpdateDepartmentRole(departmentId, CurrentUser.UserId, ids)
                };
                return Json(result);
            }

            if (userId > 0)
            {
                var result = new ResponseModel<int>
                {
                    Body = new RoleService().UpdateUserRole(userId, CurrentUser.UserId, ids)
                };
                return Json(result);
            }

            throw new TpoBaseException("参数错误");
        }

        /// <summary>
        /// 和角色关联的用户列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public PartialViewResult UserListOfRole(int roleId)
        {
            ViewBag.roleId = roleId;
            var model = new RoleService().GetUsersOfRole(roleId);
            return PartialView(model);
        }

        public JsonResult RemoveUserRole(int userId, int roleId)
        {
            var result = new ResponseModel<int>
            {
                Body = new RoleService().RemoveUserRole(userId, roleId)
            };
            return Json(result);
        }
    }
}
