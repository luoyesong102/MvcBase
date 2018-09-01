using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.Common;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Permission.Web.Models.Common;
using Esmart.Permission.Web.Models.Permissions;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web.Controllers
{
    /// <summary>
    /// 部门管理
    /// </summary>
    public class DepartmentController : BaseController
    {
        private readonly DepartmentService _departmentService;

        public DepartmentController()
        {
            _departmentService = new DepartmentService();
        }

        public ActionResult Index()
        {
            var departments = _departmentService.GetDepartments(CurrentUser.UserId, 0);

            if (departments.Count > 0)
            {
                departments[0].Open = true;
            }
            else
            {
                return View("SimpleMessage", new ModelError(@"当前用户没有所属部门!"));
            }
            departments.Add(new ZTreeNode { Name = "全部用户" });
            ViewBag.ZTreeNodes = JsonConvert.SerializeObject(departments, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return View();
        }

        #region 主页面相关

        public PartialViewResult MainContentPart(int? departmentId)
        {
            var dataSource = new DataTableSource(0,
               Url.Action("UserListPart", "Department", new { DeapartmentId = departmentId }),
               Url.Action("UserSearchPart"));
            return PartialView(DataTableSource.TableViewName, dataSource);
        }

        public PartialViewResult UserSearchPart()
        {
            return PartialView();
        }

        /// <summary>
        /// 部门用户列表
        /// </summary>
        public PartialViewResult UserListPart(int DeapartmentId, SoaDataPage<UserSearchModel> filter)
        {
            CommonFunction.InitializeSoaDataPage(filter);
            filter.Where.DeapartmentId = DeapartmentId;
            filter.Where.UserId = User.Identity.GetUserId<int>();
            var result = _departmentService.GetUsersByDepartList(filter);
            return PartialView(result);
        }

        #endregion

        #region 部门相关

        /// <summary>
        /// 添加修改部门信息
        /// </summary>
        public PartialViewResult DepartmentEdit(string act, int id)
        {
            if (act == "add")
            {
                var model = new DepartmentResponse { ParentId = id };
                return PartialView(model);
            }
            else
            {
                var model = _departmentService.GetDepartmentResponse(id);
                model.DeparentId = model.DeparentId;
                return PartialView(model);
            }
        }

        [HttpPost]
        public JsonResult SaveDepartment(DepartmentResponse model)
        {
            if (model.DeparentId == 0)
            {
                var req = new DepartmentRequest
                {
                    Name = model.Name,
                    ParentId = model.ParentId,
                    Remark = model.Remark,
                    CreateTime = DateTime.Now,
                    CreateId = CurrentUser.UserId,
                    IsDelete = 0,
                };
                model.DeparentId = _departmentService.AddDepartment(req);
            }
            else
            {
                var resp = _departmentService.GetDepartmentResponse(model.DeparentId);
                var req = new DepartmentRequest
                {
                    DeparentId = resp.DeparentId,
                    Name = model.Name,
                    Remark = model.Remark,
                    ParentId = resp.ParentId,
                    CreateTime = resp.CreateTime,
                    CreateId = resp.CreateId,
                    IsDelete = resp.IsDelete ?? 0
                };
                _departmentService.UpdateDepartment(req);
            }

            var result = new ResponseModel<DepartmentResponse>
            {
                Body = model
            };
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteDepartment(int id)
        {
            var result = new ResponseModel<int>
            {
                Body = _departmentService.DeleteDepartment(id)
            };
            if (LogHelper<DepartmentRequest>.LogAction("Esmart.Permission.Application.Data.DepartmentDbAction") != null)
            {
                LogHelper<DepartmentRequest>.LogAction("Esmart.Permission.Application.Data.DepartmentDbAction")(new UpdateUserDto { UserID = CurrentUser.UserId }, new DepartmentRequest { DeparentId=id});
                LogHelper<DepartmentRequest>.RemoveAction("Esmart.Permission.Application.Data.DepartmentDbAction");
            }
            return Json(result);
        }

        #endregion

        #region 用户相关

        public PartialViewResult UserEdit(string act, int id)
        {
            if (act == "add")
            {
                return PartialView(new UserEditViewModel { DepartmentId = id, Birthday = DateTime.Now.AddYears(-30), Sex = 0 });
            }
            var dbModel = new UserService().GetUser(id);
            var viewModel = Mapper.Map<UserEditViewModel>(dbModel);
            return PartialView(viewModel);
        }

        [HttpPost]
        public JsonResult SaveUser(UserEditViewModel viewModel)
        {
            var userService = new UserService();
            if (viewModel.UserID == 0)
            {
                //验证部门id是否存在
                if (!viewModel.DepartmentId.HasValue)
                {
                    var ret = new ResponseModel<string>
                    {
                        Header = new ResponseHeader { ReturnCode = 1, Message = "用户部门不能为空" },
                        Body = "用户部门不能为空"
                    };
                    return Json(ret);
                }
                //写入数据库
                var dbModel = Mapper.Map<Esmart_Sys_Users>(viewModel);
                dbModel.CreateTime = DateTime.Now;
                dbModel.CreateId = CurrentUser.UserId;
                dbModel.IsDelete = 0;
                userService.CreateUserWithDepartmentId(dbModel, viewModel.DepartmentId.Value);
            }
            else
            {
                var dbModel = userService.GetUser(viewModel.UserID);
                if (dbModel == null)
                {
                    throw new TpoBaseException("数据已经不存在，请刷新后重试");
                }
                Mapper.Map(viewModel, dbModel);
                userService.UpdateUser(dbModel);
            }

            var result = new ResponseModel<int>
            {
                Body = 1
            };
            return Json(result);
        }

        /// <summary>
        /// [视图]添加现有用户到部门
        /// </summary>
        public PartialViewResult AddExistingUser(int id)
        {
            var userService = new UserService();
            var users = userService.GetUserOutDepartment(id, CurrentUser.UserId);
            var model = users.Select(n => new SelectListItem { Text = n.UserName, Value = n.UserID.ToString() });
            return PartialView(model);
        }

        /// <summary>
        /// 添加现有用户到部门
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddExistingUser(int departmentId, string userIds)
        {
            if (userIds == null)
            {
                var result = new ResponseModel<string>
                {
                    Header = new ResponseHeader { ReturnCode = 1, Message = "错误：没有选择用户!" }
                };
                return Json(result);
            }
            var userService = new UserService();
            var userIdList = userIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            userService.ChangeDepartmentOfUser(userIdList, departmentId, CurrentUser.UserId);
            return Json(new ResponseModel<int> { Body = 1 });
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="depId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteUser(int userId, int depId)
        {
            var userService = new UserService();
            var result = new ResponseModel<bool>
            {
                Body = userService.DeleteUser(userId, depId)
            };
            return Json(result);
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResetUserPwd(int userId)
        {
            var userService = new UserService();
            var result = new ResponseModel<bool>
            {
                Body = userService.ResetUserPwd(userId)
            };
            return Json(result);
        }

        /// <summary>
        /// 用户离职
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserLeave(int userId)
        {
            var userService = new UserService();
            var result = new ResponseModel<bool>
            {
                Body = userService.UpdateLeaveStatus(userId)
            };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CheckUserName(string userName, string userId)
        {
            var userService = new UserService();
            var users = userService.GetUsers(new Esmart_Sys_Users { UserName = userName });
            return Json(new { code = users.Count(n => n.UserID.ToString() != userId) });
        }

        #endregion

        #region 分配角色

        public PartialViewResult AssignRole(int userId = 0, int depId = 0)
        {
            if (userId > 0)
            {
                var model = new RoleService().GetAssignRoles(CurrentUser.UserId, 0, userId);
                return PartialView(model);
            }

            if (depId > 0)
            {
                var model = new RoleService().GetAssignRoles(CurrentUser.UserId, depId, 0);
                return PartialView(model);
            }

            throw new TpoBaseException("无效的参数");
        }

        [HttpPost]
        public JsonResult AssignRoleSave(string roleIds, int userId = 0, int depId = 0)
        {
            if (roleIds == null)
            {
                roleIds = string.Empty;
            }

            var roleService = new RoleService();

            if (userId > 0)
            {
                var ids = roleIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                var result = new ResponseModel<int>
                {
                    Body = roleService.UpdateUserRole(userId, CurrentUser.UserId, ids)
                };
                return Json(result);
            }

            if (depId > 0)
            {
                var ids = roleIds.Split(',').Select(int.Parse).ToList();
                var result = new ResponseModel<int>
                {
                    Body = roleService.UpdateDepartmentRole(depId, CurrentUser.UserId, ids)
                };
                return Json(result);
            }

            throw new TpoBaseException("无效的参数");
        }
        /// <summary>
        /// 权限分配
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="depId"></param>
        /// <returns></returns>
        public PartialViewResult AssignRights(int userId = 0, int depId = 0)
        {
            if (userId > 0)
            {                
                var rolePermissions = new UserPermissionsViewModel(0, CurrentUser.UserId);
                return PartialView(rolePermissions);
            }
            throw new TpoBaseException("无效的参数");
        }

        public string GetUserPermissionTreeJson(int userId, int appId)
        {
            var roleTree = new UserPermissionsTreeViewModel(userId,CurrentUser.UserId, appId);
            return roleTree.TreeNodesJson;
        }
        public JsonResult SaveUserRights(UserPermissionsRequestModel model)
        {
            model.CreatorId = CurrentUser.UserId;
            model.CreateTime = DateTime.Now;
            model.UpdaterId = CurrentUser.UserId;
            model.UpdateTime = DateTime.Now;
            new PermissionService().AssignUserPermission(model);
            var result = new ResponseModel<string>()
            {
                Body = "保存成功！"
            };
            return Json(result);
        }
        #endregion
    }
}
