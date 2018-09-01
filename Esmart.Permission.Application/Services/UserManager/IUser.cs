using System.Collections.Generic;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application
{
    /// <summary>
    /// 用户基础信息操作
    /// </summary>
    [ServerAction(ServerType = typeof(UserManager))]
    public interface IUser
    {
        List<AppShortInfo> GetAppListByUserId(int userId);

        /// <summary>
        /// 根据用户id获取当前运用程序下面的菜单功能信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">运用程序</param>
        List<MenuResponse> GetMenuAndFunctionByUserId(int userId, int appId);

        /// <summary>
        /// 根据用户id和菜单id获取用户在当前菜单下面的功能
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="menuId">菜单编号</param>
        List<FunctionModel> GetFunctionByUserIdAndMenuId(int userId, int menuId);

        /// <summary>
        /// 根据用户信息获取菜单
        /// </summary>
        MenuResponses GetMenuByUserId(int userId, int appId);

        bool UpdateUser(UpdateUserDto user);

        bool UpdateUser2(Esmart_Sys_Users user);

        bool DeleteUser(int pm_UserID, int departMentId);

        List<Esmart_Sys_Users> GetUsers(Esmart_Sys_Users condition);

        Esmart_Sys_Users GetSingleUser(int userId);

        /// <summary>
        /// 获取当前部门以外的用户列表  by wangligui
        /// </summary>
        List<DepartmentUserResponse> GetUserOutDepartment(int departmentId, int loginUserId);

        int CreateUserWithDepartmentId(Esmart_Sys_Users user, int departMentId);

        /// <summary>
        /// 根据当前用户获取权限
        /// </summary>
        RolePermissionsResponseModel GetMenuResponses(int userId, int appId, int roleId);

        /// <summary>
        /// 设置用户的离职状态  1:离职  0：未离职
        /// </summary>
        bool UpdateLeaveStatus(int userId);

        /// <summary>
        /// 重置密码
        /// </summary>
        bool ResetUserPwd(int userid);

        /// <summary>
        /// 修改密码
        /// </summary>
        bool ModifyPassword(string userAccount, string password, string newPassword);

        bool ChangeDepartmentOfUser(List<int> userIds, int newDepartmentId, int createId);

        /// <summary>
        /// 获取用户角色列表
        /// </summary>
        List<string> GetUserRole(int userId);

        /// <summary>
        /// 根据用户ID和组名获取所有组长用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupName"></param>
        /// <param name="departId"></param>
        /// <returns></returns>
        List<UsersView> GetUsersByDepartList(int userId, string groupName, int departId = 0);

        List<UserLiteDto> GetGroupUsersByLoginUserID(int userId);
    }
}
