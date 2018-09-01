using System.Collections.Generic;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.RoleManager
{
    /// <summary>
    /// by wangligui
    /// </summary>
    [ServerAction(ServerType = typeof(RoleSoaService))]
    public interface IRoleSoaService
    {
        /// <summary>
        /// 新增   by wangligui
        /// Esmart_Sys_Roles
        /// Esmart_Sys_RolesInDeparent
        /// Esmart_Sys_Role_App
        /// </summary>
        bool AddOrUpdate(RoleModel request);

        /// <summary>
        /// 删除角色和角色对应的中间表数据    by wangligui
        /// Esmart_Sys_Roles
        /// Esmart_Sys_RolesInDeparent
        /// Esmart_Sys_Role_App
        /// Esmart_Sys_Role_Navigations
        /// Esmart_Sys_Role_Navigation_Function
        /// Esmart_Sys_User_Roles
        /// </summary>
        bool Del(RoleModel request);

        /// <summary>
        /// 更新用户角色
        /// </summary>
        int UpdateUserRole(int userId, int creatId, List<int> listRole);

        /// <summary>
        /// 获取角色信息 by  longhui 
        /// </summary>
        RoleModel GetRoleRequest(int roleid);

        /// <summary>
        /// 根据用户获取用户自己创建的角色  by  chalei.wu
        /// </summary>
        SoaDataPageResponse<RoleModel> GetRoleList(SoaDataPage<RoleGridFilterViewModel> fiter);

        /// <summary>
        /// 为部门分配角色
        /// </summary>
        int UpdateDepartmentRole(int depId, int creatId, List<int> listRole);

        List<RoleResponse> GetAssignRoles(int managerUserId, int deparentmentId, int targetUserId);

        List<DepartmentUserResponse2> GetUsersOfRole(int roleId);

        int RemoveUserRole(int userId, int roleId);

        List<FunctionResponse> GetRoleFunctions(int appId, int roleId);
    }
}
