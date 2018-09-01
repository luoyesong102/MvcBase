using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Esmart.Permission.Application.Constants;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.RoleManager
{
    public class RoleSoaService : IRoleSoaService
    {
        #region 角色添加,及相关表(Esmart_Sys_RolesInDeparent,Esmart_Sys_Role_App)

        /// <summary>
        /// 添加角色和角色部门中间表 
        /// 或修改角色
        /// Esmart_Sys_Roles
        /// Esmart_Sys_RolesInDeparent
        /// Esmart_Sys_Role_App
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        public bool AddOrUpdate(RoleModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new TpoBaseException("角色名称不能为空");

            if (request.Name.Length > 50)
                throw new TpoBaseException(string.Format(CultureInfo.CurrentCulture, "角色名称:{0} 不能超过50！", request.Name));

            if (request.Id > 0)
                return RoleDbAction.Update(request);

            var role = new Esmart_Sys_Roles()
            {
                CreateId = request.CreatorId ?? 0,
                CreateTime = DateTime.Now,
                StartTime = request.StartDate ?? DateTime.Now,
                EndTime = request.EndDate ?? DateTime.Now.AddYears(99),
                Remark = request.Remark,
                RoleName = request.Name
            };
            if (role.StartTime > role.EndTime)
                throw new TpoBaseException("角色开始时间不能大于结束时间");
            RoleDbAction.Add(role);
            return true;
        }

        #endregion

        #region 角色删除,及相关表(Esmart_Sys_Roles,Esmart_Sys_RolesInDeparent 等)

        /// <summary>
        /// 删除角色和角色对应的中间表数据
        /// Esmart_Sys_Roles
        /// Esmart_Sys_RolesInDeparent
        /// Esmart_Sys_Role_App
        /// Esmart_Sys_Role_Navigations
        /// Esmart_Sys_Role_Navigation_Function
        /// Esmart_Sys_User_Roles
        /// </summary>
        /// <param name="request">角色Id</param>
        /// <returns></returns>
        public bool Del(RoleModel request)
        {
            RoleDbAction.Delete(request.Id);
            RoleNavigationsDbAction.Del(request.Id);
            RoleNavigationFunctionDbAction.Del(request.Id);
            UserRolesDbAction.Delete(request.Id);
            DepartmentRolesDbAction.DeleteByRoleId(request.Id);
            if(LogHelper<RoleModel>.LogAction("Esmart.Permission.Application.Data.DepartmentRolesDbAction")!=null)
            {
                LogHelper<RoleModel>.LogAction("Esmart.Permission.Application.Data.DepartmentRolesDbAction")(new UserLiteDto {UserID=request.CreatorId==null?1:request.CreatorId.Value },request);
                LogHelper<RoleModel>.RemoveAction("Esmart.Permission.Application.Data.DepartmentRolesDbAction");
            }
            var remark = string.Format("用户：{0} 删除角色：{1}", request.CreatorId ?? 0, request.Id);
            LogManagerDb.Log(request.CreatorId ?? 0, DateTime.Now, remark, "RoleManager.DelByDepartmentId");

            return true;
        }

        #endregion

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="roleid">角色Id</param>
        /// <returns></returns>
        public RoleModel GetRoleRequest(int roleid)
        {
            Esmart_Sys_Roles role = RoleDbAction.Find(roleid);
            RoleModel response = new RoleModel();
            if (role != null)
            {
                response.CreatorId = role.CreateId;
                response.CreateTime = role.CreateTime;
                response.EndDate = role.EndTime;
                response.Remark = role.Remark;
                response.Id = role.RoleId;
                response.Name = role.RoleName;
                response.StartDate = role.StartTime;
                response.IsBuiltin = role.IsBuiltin;
            }
            return response;
        }

        public SoaDataPageResponse<RoleModel> GetRoleList(SoaDataPage<RoleGridFilterViewModel> fiter)
        {
            SoaDataPageResponse<RoleModel> response = new SoaDataPageResponse<RoleModel>();

            List<Esmart_Sys_Roles> roles;

            if (CommonAction.IsSysAdmin(fiter.Where.LogInUserId))
            {
                roles = RoleDbAction.GetAllRols();

                response.Count = roles.Count;

                roles = roles.Skip((fiter.PageIndex - 1) * fiter.PageSize).Take(fiter.PageSize).ToList();
            }
            else
            {
                int count;
                roles = UserRolesDbAction.GetRolesByUserId(fiter, out count);
                response.Count = count;
            }

            var body = roles.ConvertAll(a => new RoleModel()
            {
                CreateTime = a.CreateTime,
                CreatorId = a.CreateId,
                EndDate = a.EndTime,
                Id = a.RoleId,
                Name = a.RoleName,
                Remark = a.Remark,
                StartDate = a.StartTime,
                IsBuiltin = a.IsBuiltin
            });

            response.Body = body;

            return response;
        }

        /// <summary>
        /// 分配部门角色
        /// </summary>
        public int UpdateDepartmentRole(int depId, int creatId, List<int> listRole)
        {
            return RoleAssignDbAction.AssignDepartmentRoles(depId, creatId, listRole);
        }

        public int UpdateUserRole(int userId, int creatId, List<int> listRole)
        {
            UserRolesDbAction.DeleteUserRoles(userId);

            var list = new List<Esmart_Sys_User_Roles>();

            if (listRole.Any())
            {
                list.AddRange(listRole.Select(lr => new Esmart_Sys_User_Roles() { CreateId = creatId, CreateTime = DateTime.Now, RoleId = lr, UserId = userId }));
            }

            var result = UserRolesDbAction.AddList(list);
            CommonAction.ClearCache();
            return result;
        }

        /// <summary>
        /// 获取用户或部门的可分配角色列表
        /// </summary>
        public List<RoleResponse> GetAssignRoles(int managerUserId, int deparentmentId, int targetUserId)
        {
            //-------------------------------------
            // 根据管理人获取可分配角色列表
            //-------------------------------------
            var assignableRoles = CommonAction.IsSysAdmin(managerUserId)
                ? RoleDbAction.GetAllRols()
                : UserRolesDbAction.GetUserRolses(managerUserId).Where(n => !BuiltinRoles.Admins.Contains(n.RoleName)).ToList();

            //-------------------------------------
            // 检索目标用户的已分配角色列表
            //-------------------------------------
            var assignedRoles = targetUserId > 0 ? UserRolesDbAction.GetUserRolses(targetUserId) : DepartmentRolesDbAction.GetAssignedRolesOfDepartment(deparentmentId);

            //-------------------------------------
            // 已分配角色列表和可分配角色列表交叉运算
            //-------------------------------------
            var response = assignableRoles.ConvertAll(a =>
            {
                var resp = new RoleResponse()
                {
                    RoleId = a.RoleId,
                    RoleName = a.RoleName,
                    Remark = a.Remark
                };

                if (a.StartTime != null)
                    resp.StartTime = a.StartTime.Value.ToString("yyyy-MM-dd");
                if (a.EndTime != null)
                    resp.EndTime = a.EndTime.Value.ToString("yyyy-MM-dd");

                var index = assignedRoles.FindIndex(n => n.RoleId == resp.RoleId);
                if (index >= 0)
                {
                    resp.IsChoice = true;
                    assignedRoles.RemoveAt(index);
                }
                return resp;
            });

            if (assignedRoles.Count > 0)
            {
                response.InsertRange(0, assignedRoles.ConvertAll(a =>
                {
                    var resp = new RoleResponse()
                    {
                        RoleId = a.RoleId,
                        RoleName = a.RoleName,
                        Remark = a.Remark,
                        IsChoice = true
                    };

                    if (a.StartTime != null)
                        resp.StartTime = a.StartTime.Value.ToString("yyyy-MM-dd");
                    if (a.EndTime != null)
                        resp.EndTime = a.EndTime.Value.ToString("yyyy-MM-dd");
                    return resp;
                }));
            }
            return response.OrderBy(n=>n.RoleName).ToList();
        }

        public List<DepartmentUserResponse2> GetUsersOfRole(int roleId)
        {
            return UserRolesDbAction.GetUsersWithRole(roleId);
        }

        public int RemoveUserRole(int userId, int roleId)
        {
            return UserRolesDbAction.RemoveUserRole(userId, roleId);
        }

        public List<FunctionResponse> GetRoleFunctions(int appId, int roleId)
        {
            return RoleAssignDbAction.GetFunctions(appId, roleId);
        }
    }
}
